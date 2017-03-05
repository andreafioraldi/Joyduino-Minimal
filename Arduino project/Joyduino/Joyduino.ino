/* 
 * Author: Andrea Fioraldi <andreafioraldi@gmail.com>
 * License: MIT
 */
#include "PS2X_lib.h"

PS2X ps2x;

#define LOOP_DELAY 5

void (*reboot)() = 0;

void setup() {
  Serial.begin(9600);
  int error = ps2x.config_gamepad(10, 12, 11, 13, true, true);
  if (error != 0) //clock, command, attention, data, Pressures?, Rumble?
  {
    Serial.print("error\x03");
    //reboot();
    while (true) {}
  }
  else Serial.print("ready\x03");
}

void loop() {
  ps2x.read_gamepad();

  if(ps2x.NewButtonState()) {
    //pressed
    if(ps2x.ButtonPressed(PSB_PAD_UP))
         Serial.write('a');
    if(ps2x.ButtonPressed(PSB_PAD_DOWN))
         Serial.write('b');
    if(ps2x.ButtonPressed(PSB_PAD_LEFT))
         Serial.write('c');
    if(ps2x.ButtonPressed(PSB_PAD_RIGHT))
         Serial.write('d');
    if(ps2x.ButtonPressed(PSB_BLUE))
         Serial.write('e');
    if(ps2x.ButtonPressed(PSB_PINK))
         Serial.write('f');
    if(ps2x.ButtonPressed(PSB_RED))
         Serial.write('g');
    if(ps2x.ButtonPressed(PSB_GREEN))
         Serial.write('h');
    if(ps2x.ButtonPressed(PSB_L1))
         Serial.write('i');
    if(ps2x.ButtonPressed(PSB_L2))
         Serial.write('j');
    if(ps2x.ButtonPressed(PSB_R1))
         Serial.write('k');
    if(ps2x.ButtonPressed(PSB_R2))
         Serial.write('l');
    if(ps2x.ButtonPressed(PSB_SELECT))
         Serial.write('m');
    if(ps2x.ButtonPressed(PSB_START))
         Serial.write('n');
    if(ps2x.ButtonPressed(PSB_R3))
         Serial.write('o');
    if(ps2x.ButtonPressed(PSB_L3))
         Serial.write('p');
    Serial.print("\x03");
    
    //released
    if(ps2x.ButtonReleased(PSB_PAD_UP))
         Serial.write('a');
    if(ps2x.ButtonReleased(PSB_PAD_DOWN))
         Serial.write('b');
    if(ps2x.ButtonReleased(PSB_PAD_LEFT))
         Serial.write('c');
    if(ps2x.ButtonReleased(PSB_PAD_RIGHT))
         Serial.write('d');
    if(ps2x.ButtonReleased(PSB_BLUE))
         Serial.write('e');
    if(ps2x.ButtonReleased(PSB_PINK))
         Serial.write('f');
    if(ps2x.ButtonReleased(PSB_RED))
         Serial.write('g');
    if(ps2x.ButtonReleased(PSB_GREEN))
         Serial.write('h');
    if(ps2x.ButtonReleased(PSB_L1))
         Serial.write('i');
    if(ps2x.ButtonReleased(PSB_L2))
         Serial.write('j');
    if(ps2x.ButtonReleased(PSB_R1))
         Serial.write('k');
    if(ps2x.ButtonReleased(PSB_R2))
         Serial.write('l');
    if(ps2x.ButtonReleased(PSB_SELECT))
         Serial.write('m');
    if(ps2x.ButtonReleased(PSB_START))
         Serial.write('n');
    if(ps2x.ButtonReleased(PSB_R3))
         Serial.write('o');
    if(ps2x.ButtonReleased(PSB_L3))
         Serial.write('p');
    Serial.print("\x03");
  }
  else Serial.print("\x03\x03");
  
  Serial.write(ps2x.Analog(PSS_LX));
  Serial.write(ps2x.Analog(PSS_LY));
  Serial.write(ps2x.Analog(PSS_RX));
  Serial.write(ps2x.Analog(PSS_RY));
  //Serial.println();
  
  delay(LOOP_DELAY);
}
