#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <SPI.h>
#include <MFRC522.h>

const char* ssid = "FAOZAN"; // Ganti dengan SSID Wi-Fi Anda
const char* password = "@Bhakti135"; // Ganti dengan kata sandi Wi-Fi Anda
const char* serverHost = "192.168.1.2"; // Ganti dengan alamat IP server VB.NET
const int serverPort = 12345; // Ganti dengan port server VB.NET

// Pin untuk RC522
#define SS_PIN D8
#define RST_PIN D0

// Pin untuk Buzzer
#define BUZZER_PIN D1

MFRC522 mfrc522(SS_PIN, RST_PIN); // Inisialisasi RC522

void setup() {
    Serial.begin(115200);
    SPI.begin();
    mfrc522.PCD_Init();

    pinMode(LED_BUILTIN, OUTPUT);
    pinMode(BUZZER_PIN, OUTPUT);

    Serial.println();
    Serial.println("Menghubungkan ke Wi-Fi...");
    WiFi.begin(ssid, password);

    while (WiFi.status() != WL_CONNECTED) {
        delay(500);
        Serial.print(".");
    }
    Serial.println("");
    Serial.println("Terkoneksi ke Wi-Fi");
    Serial.println("Alamat IP ESP8266: ");
    Serial.println(WiFi.localIP());
}

void loop() {
    if (WiFi.status() == WL_CONNECTED) {
        if (mfrc522.PICC_IsNewCardPresent()) {
            if (mfrc522.PICC_ReadCardSerial()) {
                String uid = "";
                for (byte i = 0; i < mfrc522.uid.size; i++) {
                    uid += String(mfrc522.uid.uidByte[i], HEX);
                }

                Serial.println("Kartu terdeteksi dengan UID: " + uid);

                if (sendUIDtoServer(uid)) {
                    Serial.println("UID berhasil dikirim ke server");
                    // Bunyikan buzzer jika berhasil mengirim
                    bunyikanBuzzer();
                } else {
                    Serial.println("Gagal mengirim UID ke server");
                    // Nyalakan LED built-in selama 100 ms saat gagal mengirim
                    digitalWrite(LED_BUILTIN, HIGH);
                    digitalWrite(BUZZER_PIN, HIGH);
                    delay(10);
                    digitalWrite(LED_BUILTIN, LOW);
                    digitalWrite(BUZZER_PIN, LOW);
                }
            }
            mfrc522.PICC_HaltA();
        }
    } else {
        Serial.println("Tidak terhubung ke Wi-Fi");
    }
    delay(100); // Tunda 1 detik untuk mengurangi kecepatan pembacaan kartu
}

bool sendUIDtoServer(String uid) {
    WiFiClient client;
    
    if (!client.connect(serverHost, serverPort)) {
        return false;
    }

    client.print(uid);
    client.stop();
    return true;
}

void bunyikanBuzzer() {
    // Bunyikan buzzer dengan frekuensi 1000 Hz selama 100 ms
    tone(BUZZER_PIN, 1500, 100);
}
