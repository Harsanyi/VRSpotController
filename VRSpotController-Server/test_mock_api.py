"""Regression checks for the hardware-free API contract. No robot connection is made."""
import inspect
import json
import os
import unittest
from pathlib import Path
from unittest.mock import patch

from api import create_app
from server import Server
from spot.fakeSpot import FakeSpot
from spot.spot import Spot
from vector import vector2, vector3, vector4


class MockApiTests(unittest.TestCase):
    def setUp(self):
        self.app = create_app(debug=True)
        self.client = self.app.test_client()

    def post(self, path, payload):
        response = self.client.post(path, json=payload)
        self.assertEqual(response.status_code, 200)
        data = json.loads(response.data)
        self.assertTrue(data['success'], data)
        return data

    @patch('spot.fakeSpot.time.sleep')
    def test_all_command_endpoints_and_state(self, sleep):
        self.assertEqual(self.client.get('/').status_code, 200)
        self.assertIn('Hello', self.post('/hello', {'name': 'Test'})['message'])
        self.post('/power', {'powerOn': True})
        self.post('/stand', {'stand': True})
        self.post('/move', {'move': {'x': 1, 'y': -0.5}, 'yaw': 0.2})
        self.post('/pitch', {'pitch': 0.1})
        self.post('/grip', {'open': 0.5})
        self.post('/arm', dict(posX=0.2, posY=0.3, posZ=0.4, rotX=0, rotY=0, rotZ=0, rotW=1))
        state = self.post('/state', {})['state']
        self.assertEqual(state['powerState'], 2)
        self.assertEqual(state['poseState'], 4)
        self.assertAlmostEqual(state['posX'], 0.1)
        self.assertAlmostEqual(state['posY'], -0.05)
        self.assertEqual(state['armRotW'], 1)
        self.assertEqual(state['armPosZ'], 0.4)
        self.assertTrue(0 <= state['battery'] <= 100)
        self.post('/move', {'move': {'x': 0, 'y': 0}, 'yaw': 0})
        self.assertEqual(self.post('/state', {})['state']['poseState'], 3)
        self.post('/stand', {'stand': False})
        self.post('/power', {'powerOn': False})
        state = self.post('/state', {})['state']
        self.assertEqual((state['poseState'], state['powerState']), (1, 1))

    def test_all_camera_sources_from_another_working_directory(self):
        for source in ('back_fisheye_image', 'frontleft_fisheye_image', 'frontright_fisheye_image',
                       'left_fisheye_image', 'right_fisheye_image', 'hand_color_image'):
            with self.subTest(source=source):
                response = self.client.post('/image', json={'source': source})
                self.assertEqual(response.status_code, 200)
                self.assertEqual(response.mimetype, 'image/jpeg')
                self.assertTrue(response.data.startswith(b'\xff\xd8'))
                expected = Path(__file__).parent / (source + '.jpg')
                self.assertEqual(response.data, expected.read_bytes())
        self.assertEqual(self.client.post('/image', json={'source': 'missing'}).status_code, 404)

    def test_backend_contract_and_instance_isolation(self):
        for name in ('Move', 'Pitch', 'StandUp', 'SitDown', 'PowerOn', 'DropLease',
                     'GetImage', 'GetState', 'SetGripper', 'SetArm'):
            self.assertEqual(list(inspect.signature(getattr(Spot, name)).parameters),
                             list(inspect.signature(getattr(FakeSpot, name)).parameters))
        first, second = Server(debug=True).spot, Server(debug=True).spot
        first.Move(vector2(1, 0), 0)
        self.assertEqual(second.position.x, 0)
        first.SetGripper(2)
        self.assertEqual(first.gripper, 1)
        self.assertEqual(second.gripper, 0)
        first.SetArm(vector3(1, 2, 3), vector4(0, 0, 0, 1))
        self.assertEqual(first.GetState().armRotW, 1)

    def test_real_mode_requires_credentials_without_connecting(self):
        with patch.dict(os.environ, {}, clear=True):
            with self.assertRaisesRegex(RuntimeError, 'SPOT_IP, SPOT_USERNAME, SPOT_PASSWORD'):
                Server(debug=False)

    def test_state_failure_is_reported(self):
        with patch.object(FakeSpot, 'GetState', side_effect=RuntimeError('state unavailable')):
            response = self.client.post('/state', json={})
            data = json.loads(response.data)
            self.assertFalse(data['success'])
            self.assertEqual(data['message'], 'state unavailable')


if __name__ == '__main__':
    unittest.main()
