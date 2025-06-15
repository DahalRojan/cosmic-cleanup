using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class DataLogger : MonoBehaviour
{
    private List<LogEntry> logEntries = new List<LogEntry>();
    private const int MaxEntries = 1000;

    struct LogEntry
    {
        public float time;
        public Vector3 position;
        public Vector3 rotation;
        public float speed;
        public float totalDistance;
        public float mouseDeltaX;
        public float mouseDeltaY;
        public float rotationalSpeed;
        public float acceleration;
        public float headSwayAngle;
        public Vector3 simulatedAcceleration; // Accelerometer data
        public Vector3 simulatedAngularVelocity; // Gyroscope data

        public LogEntry(float t, Vector3 pos, Vector3 rot, float spd, float dist, float deltaX, float deltaY, float rotSpeed, float accel, float sway, Vector3 simAccel, Vector3 simAngVel)
        {
            time = t;
            position = pos;
            rotation = rot;
            speed = spd;
            totalDistance = dist;
            mouseDeltaX = deltaX;
            mouseDeltaY = deltaY;
            rotationalSpeed = rotSpeed;
            acceleration = accel;
            headSwayAngle = sway;
            simulatedAcceleration = simAccel;
            simulatedAngularVelocity = simAngVel;
        }
    }

    public void LogData(float time, Vector3 position, Vector3 rotation, float speed, float totalDistance, float mouseDeltaX, float mouseDeltaY, float rotationalSpeed, float acceleration, float headSwayAngle, Vector3 simulatedAcceleration, Vector3 simulatedAngularVelocity)
    {
        logEntries.Add(new LogEntry(time, position, rotation, speed, totalDistance, mouseDeltaX, mouseDeltaY, rotationalSpeed, acceleration, headSwayAngle, simulatedAcceleration, simulatedAngularVelocity));
        if (logEntries.Count > MaxEntries)
        {
            logEntries.RemoveRange(0, logEntries.Count - MaxEntries);
        }
    }

    public async void WriteToFile()
    {
        string path = Application.dataPath + "/PlayerData.csv";
        await Task.Run(() =>
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("Time,PosX,PosY,PosZ,RotX,RotY,RotZ,Speed,TotalDistance,MouseDeltaX,MouseDeltaY,RotationalSpeed,Acceleration,HeadSwayAngle,SimAccelX,SimAccelY,SimAccelZ,SimAngVelX,SimAngVelY,SimAngVelZ");
                foreach (var entry in logEntries)
                {
                    writer.WriteLine(
                        entry.time + "," +
                        entry.position.x + "," +
                        entry.position.y + "," +
                        entry.position.z + "," +
                        entry.rotation.x + "," +
                        entry.rotation.y + "," +
                        entry.rotation.z + "," +
                        entry.speed + "," +
                        entry.totalDistance + "," +
                        entry.mouseDeltaX + "," +
                        entry.mouseDeltaY + "," +
                        entry.rotationalSpeed + "," +
                        entry.acceleration + "," +
                        entry.headSwayAngle + "," +
                        entry.simulatedAcceleration.x + "," +
                        entry.simulatedAcceleration.y + "," +
                        entry.simulatedAcceleration.z + "," +
                        entry.simulatedAngularVelocity.x + "," +
                        entry.simulatedAngularVelocity.y + "," +
                        entry.simulatedAngularVelocity.z
                    );
                }
            }
        });
        Debug.Log("Data saved to " + path);
        logEntries.Clear();
    }
}