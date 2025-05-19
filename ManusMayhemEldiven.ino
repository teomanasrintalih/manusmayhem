#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "MPU6050.h"
#include "Wire.h"
#include "BluetoothSerial.h" 

BluetoothSerial SerialBT; 

MPU6050 mpu;

#define OUTPUT_READABLE_QUATERNION

#define INTERRUPT_PIN 2
#define LED_PIN 12
bool blinkState = false;

bool dmpReady = false;
uint8_t mpuIntStatus;
uint8_t devStatus;
uint16_t packetSize;
uint16_t fifoCount;
uint8_t fifoBuffer[64];

Quaternion q;
VectorInt16 aa;
VectorInt16 aaReal;
VectorInt16 aaWorld;
VectorFloat gravity;
float euler[3];
float ypr[3];

uint8_t teapotPacket[14] = { '$', 0x02, 0, 0, 0, 0, 0, 0, 0, 0, 0x00, 0x00, '\r', '\n' };

volatile bool mpuInterrupt = false;
void dmpDataReady() {
  mpuInterrupt = true;
}

float RateRoll, RatePitch, RateYaw;
float AccX, AccY, AccZ;
float AngleRoll, AnglePitch;
float LoopTimer;

int Min_isaret, Min_orta, Min_yuzuk, Min_serce, Min_bas;
int Max_isaret = 50, Max_orta = 50, Max_yuzuk = 50, Max_serce = 80, Max_bas = 50;

void setup() {
  Wire.begin();
  Serial.begin(115200); // USB için (debug vs)
  SerialBT.begin("ManusMayhemEldiven"); // <<< Bluetooth ismini buradan ayarlıyoruz
  Serial.println("Bluetooth başlatıldı: ManusMayhemEldiven");

  mpu.initialize();
  pinMode(INTERRUPT_PIN, INPUT);

  devStatus = mpu.dmpInitialize();

  mpu.setXGyroOffset(108);
  mpu.setYGyroOffset(36);
  mpu.setZGyroOffset(48);
  mpu.setXAccelOffset(-1585);
  mpu.setYAccelOffset(3611);
  mpu.setZAccelOffset(765);

  if (devStatus == 0) {
    mpu.CalibrateAccel(6);
    mpu.CalibrateGyro(6);
    mpu.PrintActiveOffsets();
    mpu.setDMPEnabled(true);

    attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
    mpuIntStatus = mpu.getIntStatus();
    dmpReady = true;
    packetSize = mpu.dmpGetFIFOPacketSize();
  } else {
    Serial.print(F("DMP Initialization failed (code "));
    Serial.print(devStatus);
    Serial.println(F(")"));
  }

  pinMode(LED_PIN, OUTPUT);
  pinMode(34, INPUT);
  pinMode(35, INPUT);
  pinMode(32, INPUT);
  pinMode(33, INPUT);
  pinMode(25, INPUT);
}

void loop() {
// Flex sensörlerden okuma (her parmak için doğru pinlere atanmıştır)
int serce = analogRead(34);   // flex 5
int yuzuk = analogRead(25);   // flex 4
int orta  = analogRead(32);   // flex 3
int isaret = analogRead(33);  // flex 2
int bas   = analogRead(35);   // flex 1

// Her flex sensörün min-max değerlerini deneme koduyla ölçün ve aşağıya uygun şekilde girin.
// Not: map fonksiyonundaki 3400, 3100 vb. değerler örnektir, kendi ölçümünüzle değiştirin. (Githubdaki projede ölçüm kodları da mevcut.)

serce  = map(serce,  3400, 3100, Max_serce,  Min_serce);  // <- flex 5 (serçe parmak)
yuzuk  = map(yuzuk,  3400, 3100, Max_yuzuk,  Min_yuzuk);  // <- flex 4 (yüzük parmak)
orta   = map(orta,   2850, 2690, Max_orta,   Min_orta);   // <- flex 3 (orta parmak)
isaret = map(isaret, 2850, 2690, Max_isaret, Min_isaret); // <- flex 2 (işaret parmak)
bas    = map(bas,    2700, 2520, Max_bas,    Min_bas);    // <- flex 1 (baş parmak)


  if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer)) {
    mpu.dmpGetQuaternion(&q, fifoBuffer);
    SerialBT.print(q.w); SerialBT.print(",");
    SerialBT.print(q.x); SerialBT.print(",");
    SerialBT.print(q.y); SerialBT.print(",");
    SerialBT.print(q.z); SerialBT.print(",");
  }

  SerialBT.print(serce); SerialBT.print(",");
  SerialBT.print(yuzuk); SerialBT.print(",");
  SerialBT.print(orta); SerialBT.print(",");
  SerialBT.print(isaret); SerialBT.print(",");
  SerialBT.println(bas);

  delay(50);
}
