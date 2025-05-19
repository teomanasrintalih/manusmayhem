// Flex sensör pinleri
const int flexPins[4] = {25, 33, 32, 35};

// Kalibrasyon değerleri (örnek)
int minVal[4] = {2000, 2000, 2000, 2000};  // düzkenki değer
int maxVal[4] = {3200, 3200, 3200, 3200};  // bükülmüşkenki değer

void setup() {
  Serial.begin(115200);
  delay(1000);
  Serial.println("Flex sensör başlatıldı...");
}

void loop() {
  for (int i = 0; i < 4; i++) {
    int raw = analogRead(flexPins[i]);

    // Değeri dereceye çevir (örnek: 0° düz, 90° tam bükülmüş)
    int degree = map(raw, minVal[i], maxVal[i], 0, 90);
    degree = constrain(degree, 0, 90); // 0-90 derece aralığında sınırlama

    Serial.print("Flex ");
    Serial.print(i + 1);
    Serial.print(": ");
    Serial.print(raw);
    Serial.print(" -> ");
    Serial.print(degree);
    Serial.println("°");
  }

  Serial.println("---------------");
  delay(300); // okuma sıklığı
}
