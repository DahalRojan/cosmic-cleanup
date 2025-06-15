using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;


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
        // 1) Generate a session ID
        string sessionID = Guid.NewGuid().ToString();

        // 2) Build the CSV path
        string path = Application.dataPath + "/PlayerData.csv";

        // 3) Do the file I/O on a background thread
        await Task.Run(() =>
        {
            // 4) Declare 'writer' here, so it's in scope for everything below
            using (StreamWriter writer = new StreamWriter(path))
            {
                // 5) Write session metadata as first line
                writer.WriteLine("SessionID," + sessionID);

                // 6) Write your CSV header
                writer.WriteLine("Time,PosX,PosY,PosZ,RotX,RotY,RotZ,Speed,TotalDistance,MouseDeltaX,MouseDeltaY,RotationalSpeed,Acceleration,HeadSwayAngle,SimAccelX,SimAccelY,SimAccelZ,SimAngVelX,SimAngVelY,SimAngVelZ");

                // 7) Write each logged entry
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

        // 8) After saving, clear the buffer & log success
        Debug.Log("Data saved to " + path);
        logEntries.Clear();
    }

}