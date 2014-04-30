#include "Arduino.h"
#include "Joystick.h"

void Joystick::begin()
{
  Serial.begin(9600);
}

void Joystick::sendData(float xvel, float yvel, boolean leftclick, boolean rightclick, boolean crouch, boolean w, boolean a, boolean s, boolean d)
{
  Serial.print(xvel, 6);
  Serial.print(',');
  Serial.print(yvel, 6);
  Serial.print(',');
  byte packet = 0;
  packet += leftclick  << 0;
  packet += rightclick << 1;
  packet += crouch     << 2;
  packet += w          << 3;
  packet += a          << 4;
  packet += s          << 5;
  packet += d          << 6;
  Serial.println(packet, HEX);
}
