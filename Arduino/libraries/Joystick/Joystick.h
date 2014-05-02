#ifndef JOYSTICK_H
#define JOYSTICK_H

#include "Arduino.h"

class Joystick
{
public:
  static void begin();
  static void sendData(float xvel, float yvel, boolean leftclick, boolean rightclick, boolean crouch, boolean w, boolean a, boolean s, boolean d, boolean jump);
};

#endif
