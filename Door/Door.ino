#include <SPI.h>
#include <UIPEthernet.h>
//#include <Ethernet.h>
#include <PubSubClient.h>
#include <Keypad.h>

//door/password   /d/p
//door/hello      /d/h
//door/open       /d/o

#define PASS_LENGHT 10
#define DEBUG 

byte mac[] = { 0xAA, 0xED, 0xBA, 0xFE, 0xFE, 0xED };
char password[PASS_LENGHT];
char cPassword[PASS_LENGHT];
short int cPasswordCursor;
unsigned long cPasswordLastTime;
unsigned long cOpenTime;
unsigned long cEhloTime;
IPAddress mqttServer(192, 168, 88, 250);

EthernetClient ethClient;
PubSubClient client(ethClient);


void mqttMessages(char* topic, byte* payload, unsigned int length)
   {
	 if(strcmp(topic, "/d/p") == 0)
	    {
      if(length>PASS_LENGHT-1)return;
		  memcpy(password, payload, length);
		  password[length] = '\0';
	    }
   else if(strcmp(topic,"/d/o")==0)
      {
      cOpenTime = millis();
      analogWrite(A0, 255);
      }
   }

void reconnect() 
   {
   while(!client.connected()) 
      {
      if(client.connect("d")) 
         {
#ifdef DEBUG          
         Serial.println("mqttc");
#endif
         client.publish("/d/h", "hw");
         client.subscribe("/d/#");
         }
      else
         {
#ifdef DEBUG          
         Serial.println("e2");
#endif         
         delay(5000);
         }
      }
   }

char keys[4][4] = {
	{ '1','2','3','A' },
	{ '4','5','6','B' },
	{ '7','8','9','C' },
	{ '*','0','#','D' }
};
byte rowPins[4] = { 2, 3, 4, 5 };
byte colPins[4] = { 6, 7, 8, 9 };
Keypad keypad = Keypad(makeKeymap(keys), rowPins, colPins, 4, 4);

void setup()
   {
   pinMode(A0,OUTPUT);
   analogWrite(A0,0);
   cOpenTime=0;
   cEhloTime=0;
   password[0]='\0';
	 cPasswordCursor = 0;
#ifdef DEBUG  
	 Serial.begin(9600);
	 while (!Serial) {}
	 Serial.println("s");
#endif  
	 client.setServer(mqttServer,1883);
	 client.setCallback(mqttMessages);
	 while(Ethernet.begin(mac)==0)
	    {
#ifdef DEBUG      
      Serial.println("e1");
#endif      
		  delay(5000);
	    }
#ifdef DEBUG
	 Serial.print("IP");
	 Serial.println(Ethernet.localIP());
#endif
   //wdt_enable(WDTO_4S);
   }

void loop()
   {
    //wdt_reset();
   if((millis()-cEhloTime)>30000)
      {
      cEhloTime=millis();
      client.publish("/d/h", "hw");    
      }
	 if(!client.connected()) 
	    {
		  reconnect();
	    }
	 client.loop();
	 char key=keypad.getKey();
   if(cOpenTime!=0)
      {
      unsigned long cTime=millis();
      if((cTime-cOpenTime)>3000)
         {
#ifndef DEBUG          
         Serial.println("close door");
#endif         
         analogWrite(A0,0);
         cOpenTime=0;
         }
      cPasswordCursor=0;
      }
   else if(key!=NO_KEY)
	     {
#ifdef DEBUG        
       Serial.println(key);
#endif       
		   if(cPasswordCursor==PASS_LENGHT-1)
		      {
			    cPasswordCursor=0;
		      }
		   cPassword[cPasswordCursor]=key;
		   cPassword[cPasswordCursor+1]='\0';
		   cPasswordLastTime=millis();
		   cPasswordCursor++;
	     }
   else if (cPasswordCursor > 0)
	    {
		  unsigned long delay=millis()-cPasswordLastTime;
		  if (delay>2000 || strlen(password)==cPasswordCursor)
		     {
			   if (strcmp(cPassword,password)==0)
			      {
				    cOpenTime=millis();
#ifdef DEBUG           
            Serial.println("open door");
#endif            
				    analogWrite(A0, 255);
			      }
			   cPasswordCursor=0;
		     }
	    }
   }
