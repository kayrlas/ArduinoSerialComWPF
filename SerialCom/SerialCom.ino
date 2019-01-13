// copyright Kayrlas(https://github.com/kayrlas)

int message = 0;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  if (Serial.available() > 0)
  {
    message = Serial.read();
    
    Serial.print("Recieved: ");
    Serial.println((char)message);
  }
  delay(500);
}
