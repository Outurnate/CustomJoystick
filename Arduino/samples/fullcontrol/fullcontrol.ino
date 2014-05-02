#include <Joystick.h>

#define YMIN 450
#define YMAX 1023
#define XMIN 161
#define XMAX 690
#define XCENTER 526
#define YCENTER 815

#define XPIN 0
#define YPIN 1
#define CLICKPIN 13
#define RIGHTCLICKPIN 12
#define CROUCH 11
#define W 10
#define A 9
#define S 8
#define D 7
#define JUMP 6

void setup()
{
  Joystick::begin();
  pinMode(CLICKPIN, INPUT);
  pinMode(RIGHTCLICKPIN, INPUT);
  pinMode(CROUCH, INPUT);
  pinMode(W, INPUT);
  pinMode(A, INPUT);
  pinMode(S, INPUT);
  pinMode(D, INPUT);
  pinMode(JUMP, INPUT);
}

void loop()
{
  float x = (float)(analogRead(XPIN) - XCENTER) / (float)(XMAX - XMIN);
  float y = (float)(analogRead(YPIN) - YCENTER) / (float)(YMAX - YMIN);
  Joystick::sendData(x, y, digitalRead(CLICKPIN), digitalRead(RIGHTCLICKPIN), digitalRead(CROUCH), digitalRead(W), digitalRead(A), digitalRead(S), digitalRead(D), digitalRead(JUMP));
}
