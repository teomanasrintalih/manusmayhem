#include <Wire.h>
#include <BluetoothSerial.h>
#include "MPU6050_6Axis_MotionApps20.h"

BluetoothSerial SerialBT;
MPU6050 mpu;

Quaternion q;
uint8_t fifoBuffer[64];
uint16_t packetSize;

bool dmpReady = false;

void setup() {
  Wire.begin();
  Serial.begin(115200); // USB üzerinden debug
  SerialBT.begin("ManusMayhemCAM"); // Bluetooth cihaz adı

  Serial.println("Bluetooth başlatıldı: ManusMayhemCAM");

  mpu.initialize();

  if (mpu.dmpInitialize() == 0) {
    mpu.setXGyroOffset(108);
    mpu.setYGyroOffset(36);
    mpu.setZGyroOffset(48);
    mpu.setXAccelOffset(-1585);
    mpu.setYAccelOffset(3611);
    mpu.setZAccelOffset(765);

    mpu.setDMPEnabled(true);
    dmpReady = true;
    packetSize = mpu.dmpGetFIFOPacketSize();

    Serial.println("DMP hazır");
  } else {
    Serial.println("DMP başlatılamadı!");
  }
}

void loop() {
  if (!dmpReady) return;

  if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {
    mpu.dmpGetQuaternion(&q, fifoBuffer);

    String output = String(q.w, 4) + "," + 
                    String(q.x, 4) + "," + 
                    String(q.y, 4) + "," + 
                    String(q.z, 4);

    SerialBT.println(output);  
    delay(50);                
  }
}
