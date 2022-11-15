//*
#include <SoftwareSerial.h>

SoftwareSerial mySerial(10,11); // RX, TX

void setup() {
  Serial.begin(4800); // open the serial port at 9600 bps:
  while (!Serial) {
    ; // wait for serial port to connect. Needed for Native USB only
  }
  Serial.println("Goodnight moon!");
  mySerial.begin(4800);
}

void loop() {
if(mySerial.available())
  {
  //char c=mySerial.read();
  Serial.println(mySerial.read(),HEX);
  }
}

/*/

#define FUGPS_DEBUG

#include <NeoSWSerial.h>
#include <FuGPS.h>

NeoSWSerial in(10,11);
FuGPS fuGPS(in);

volatile uint32_t newlines = 0UL;
bool gpsAlive = false;
    
static void handleRxChar( uint8_t c )
  {
  Serial.print(c);    
  if (c == '\n')
    newlines++;
  }

void setup() {
  Serial.begin(9600); // open the serial port at 9600 bps:
  while (!Serial) {
    ; // wait for serial port to connect. Needed for Native USB only
  }
  Serial.println("Goodnight moon!");
  in.begin(9600);
//  in.attachInterrupt( handleRxChar );
  //fuGPS.sendCommand(FUGPS_PMTK_SET_NMEA_BAUDRATE_9600);
  //fuGPS.sendCommand(FUGPS_PMTK_API_SET_NMEA_OUTPUT_RMCGGA);
}
char _currentBuff[100];
int _bufferIndex=0;

void loop() {
   if (in.available())
    {
        char c = in.read();
        _currentBuff[_bufferIndex]=c;
        if(_bufferIndex==99)
          {
          Serial.print(_currentBuff);
          _bufferIndex=0;
          }
         else
         {
          _bufferIndex++;
         }
    }

if (fuGPS.read())
    {
        byte tokenIdx = 0;
        while (const char * token = fuGPS.getField(tokenIdx++))
        {
            Serial.print("Token [" + String(tokenIdx) + "]: ");
            Serial.println(token);
        }
        Serial.println();
    } 
  if (fuGPS.isAlive() == false)
  {
      if (gpsAlive == true)
      {
          gpsAlive = false;
          Serial.println("GPS module not responding with valid data.");
          Serial.println("Check wiring or restart.");
      }
  }     
  
}
//*/
