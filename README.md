# 🤖 BrainBot Analyse & Bemaßung v1.0.2

<table>
  <tr>
    <td><img src="./images/robot.jpg" alt="BrainBot Dashboard" width="400"/></td>
    <td><img src="./images/dashboard.png" alt="Zweites Bild" width="400"/></td>
  </tr>
</table>

Willkommen beim BrainBot-Steuerungssystem!
Dieses Projekt ermöglicht die Fernsteuerung und Analyse eines Roboters über ein modernes Web-Dashboard, die Speicherung und Auswertung von Sensordaten, KI-gestützte Kategorisierung sowie den Export technischer Berichte.
**Das System läuft standardmäßig im Simulationsmodus** – auch ohne echte Hardware.

---


# Unterrichtsbeispiel für objektorientierte Programmierung in C#

**Dieses Projekt ist speziell für den Einsatz im Unterricht und in der Hochschullehre konzipiert. Es dient als praxisnahes Beispiel für objektorientierte Programmierung in C# und demonstriert zentrale OOP-Konzepte anhand eines modularen Robotersystems.**

## Hauptbestandteil: Das UnterrichtsBeispiel-Verzeichnis

Im Verzeichnis `UnterrichtsBeispiel` befindet sich die Datei `DidaktikBeispiel.cs`, die als zentrales didaktisches Beispiel für den Unterricht dient. Die Struktur ist so gewählt, dass sie die wichtigsten Prinzipien der objektorientierten Softwareentwicklung anschaulich und nachvollziehbar vermittelt und direkt im Unterricht eingesetzt werden kann.

### Didaktische Gliederung und Unterrichtsideen

- **1. Aufbau & Inbetriebnahme (Simulation):** Einstieg ohne Hardware, alle Methoden laufen im Simulationsmodus. Ideal für erste Experimente und zum Kennenlernen der Projektstruktur.

- **2. MotorController:** Kapselt die Motorsteuerung. Im Unterricht kann hier die Bedeutung von Kapselung, Modularisierung und klaren Schnittstellen behandelt werden. Aufgabenidee: Implementiere weitere Fahrbefehle (z.B. Rückwärtsfahren, Drehen).

- **3. SensorModule:** Abstraktion verschiedener Sensoren (z.B. Abstandssensor). Lehrende können zeigen, wie lose Kopplung und Erweiterbarkeit durch Interfaces oder Vererbung erreicht werden. Aufgabenidee: Simuliere weitere Sensoren (z.B. Liniensensor, Temperatursensor).

- **4. Robot:** Komposition – die zentrale Klasse, die MotorController und SensorModule zusammenführt. Hier kann die Zusammenarbeit mehrerer Module und die Umsetzung komplexerer Logik (z.B. „Fahre bis Hindernis erkannt“) erläutert werden. Aufgabenidee: Baue eine Methode „Fahre Quadrat“ oder „Folge einer Linie“.

- **5. Erweiterung: KI/Simulation:** Die KI-Logik kann als separates Modul eingebunden werden (z.B. ML.NET, Zufallsentscheidungen, etc.). Aufgabenidee: Integriere eine einfache Entscheidungslogik oder binde ein ML.NET-Modell ein.

- **6. Hauptprogramm:** Einstiegspunkt für die Simulation. Hier werden die Module instanziiert und das Gesamtsystem gestartet. Ideal, um den Zusammenhang zwischen Theorie und Praxis zu zeigen.

### Konkrete Unterrichtsaufgaben und -ziele

- **Refactoring:** Baue die Klassen so um, dass echte Hardware angebunden werden kann (Dependency Injection, Interfaces).
- **Testen:** Schreibe Unit-Tests für MotorController und SensorModule.
- **Erweiterung:** Ergänze neue Fahrmodi oder Sensoren und diskutiere die Auswirkungen auf die Architektur.
- **Fehlerbehandlung:** Implementiere Fehlerfälle (z.B. Sensor defekt) und diskutiere Exception Handling.
- **Dokumentation:** Kommentiere die Klassen und Methoden nach C#-Standard.

### Integration von Swagger UI im Unterricht

- **API-Exploration:** Mit Swagger UI ([http://localhost:4000/swagger](http://localhost:4000/swagger)) können die REST-Endpunkte des Projekts live ausprobiert werden. Das ist ideal, um die Kommunikation zwischen Frontend, Backend und Simulation zu demonstrieren.
- **Aufgabenidee:** Sende eigene Kommandos an den Roboter über Swagger UI und beobachte die Auswirkungen in der Simulation und im Dashboard.
- **API-Dokumentation:** Zeige, wie OpenAPI/Swagger die Schnittstellen dokumentiert und wie daraus automatisch Client-Code generiert werden kann.

### Beispielhafter Ablauf für eine Unterrichtseinheit

1. **Vorstellung der Projektstruktur und OOP-Konzepte anhand von DidaktikBeispiel.cs**
2. **Live-Demo:** Simulation starten, Fahrbefehl über Swagger UI senden, Ergebnis im Dashboard beobachten
3. **Code-Analyse:** Klassenstruktur, Modularisierung, Erweiterbarkeit
4. **Selbstständige Aufgaben:** Neue Fahrbefehle/Sensoren implementieren, Tests schreiben, API erweitern
5. **Reflexion:** Vorteile von OOP, Testbarkeit, Dokumentation, API-Design

Jede dieser Komponenten ist so gestaltet, dass sie im Unterricht einzeln behandelt, erweitert und getestet werden kann. Das fördert das Verständnis für saubere Softwarearchitektur und die Prinzipien der objektorientierten Programmierung.

---

## 🚀 Features

- **Web-Dashboard:** Live-Visualisierung der Roboterposition auf einem 2D-Gitter, Tabellenansicht, Exportfunktionen.
- **Simulationsmodus:** Vollständige Funktionalität ohne Hardware – alle Bewegungen, Sensordaten und KI-Vorhersagen werden simuliert.
- **MSSQL-Integration:** Speicherung von Bewegungsdaten, Distanzen und Payloads in einer Microsoft SQL Server-Datenbank.
- **Technisches Zeichnen:** Automatisierte Erstellung von JPG-Exporten der Raummaße.
- **REST-API:** Steuerung, Status, Not-Aus, Export, KI-Vorhersage.
- **KI-Integration:** Live-Kategorisierung (z.B. „Flur“, „Hindernis“) per API und im Dashboard, ML.NET-ready.
- **Testautomatisierung:** Umfangreiche Unit- und Integrationstests für alle Kernfunktionen.
- **Logging:** Zentrales, thread-sicheres Logging aller Aktionen und Fehler.

---

## 🛠 Technologie-Stack

- **Backend:** .NET 8.0, ASP.NET Core Web API
- **Frontend:** HTML5, CSS3, JavaScript (Vanilla)
- **Datenbank:** Microsoft SQL Server (LocalDB oder Server)
- **KI:** ML.NET (vorbereitet, aktuell Simulationsmodus)
- **Bibliotheken:** Newtonsoft.Json, Microsoft.Data.SqlClient, System.Drawing.Common, Microsoft.ML (optional für KI)

---

## 📂 Projektstruktur

```
/Models                // Datenbank, Hardware, KI, Simulation, Logging
/Controllers           // API-Endpunkte (WebControlController)
/wwwroot/web_control   // Web-Frontend (index.html)
/wwwroot/exports       // Generierte JPG-Berichte
/Tests                 // Unit- und Integrationstests (xUnit)
README.md              // Diese Anleitung
```

---

## ⚙️ Installation & Setup

### Voraussetzungen

- **.NET 8.0 SDK** (https://dotnet.microsoft.com/download)
- **Visual Studio 2022/2026** (Community Edition reicht)
- **Microsoft SQL Server** (LocalDB oder Server, Standard-Instanz: `BrainBotAI`)
- **(Optional für KI)**: ML.NET NuGet-Paket (`Microsoft.ML`)

### Schritte

1. **Repository klonen:**
```sh
git clone https://github.com/marcus39-web/GHI-CSharp-Roboter-OOP.git
cd GHI-CSharp-Roboter-OOP
```

2. **Abhängigkeiten installieren:**
   - Öffne die Solution (`.sln`) in Visual Studio.
   - Stelle sicher, dass die NuGet-Pakete installiert sind (werden beim ersten Build automatisch geladen).

3. **Datenbank einrichten:**
   - Stelle sicher, dass ein SQL Server (LocalDB reicht) läuft.
   - Die Datenbank wird beim ersten Start automatisch angelegt und verwendet.

4. **Frontend vorbereiten:**
   - Stelle sicher, dass `index.html` auf „In Ausgabeverzeichnis kopieren: Immer kopieren“ steht.

---


## 📑 Swagger UI & API-Dokumentation

Das Projekt bietet eine automatisch generierte API-Dokumentation mit **Swagger UI** (OpenAPI). Damit können alle Endpunkte direkt im Browser getestet und dokumentiert werden.

### Swagger UI öffnen

- Nach dem Start des Projekts im Browser aufrufen:
  - [http://localhost:4000/swagger](http://localhost:4000/swagger)

- Hier können alle API-Endpunkte interaktiv ausprobiert, getestet und die Request/Response-Formate eingesehen werden.

### Wichtige Funktionen über Swagger UI

- **/api/webcontrol/command** – Sende Bewegungsbefehle (POST, JSON-Body: `command`, `posX`, `posY`, `distance`)
- **/api/webcontrol/history** – Hole die letzten Bewegungsdaten (GET)
- **/api/webcontrol/predict** – KI-Vorhersage für ein Kommando (POST)
- **/api/export** – Exportiere eine technische Zeichnung als JPG (POST)
- **/api/webcontrol/emergency-stop** – Not-Aus (POST)

Swagger/OpenAPI ist ideal für:
- Entwicklung und Testen der API
- Automatische Dokumentation
- Generierung von Client-Code (z.B. für TypeScript, C#, Python)


1. **Projekt starten:**
   - Drücke `F5` in Visual Studio oder führe `dotnet run` im Projektverzeichnis aus.

2. **Web-Dashboard öffnen:**
   - Im Browser: [http://localhost:4000](http://localhost:4000)

3. **Funktionen im Dashboard:**
   - **Live-Visualisierung:** Bewegungen und Positionen werden auf dem Gitter angezeigt.
   - **Tabellenansicht:** Alle Bewegungsdaten, Sensordaten und KI-Kategorien.
   - **JPG-Export:** Klick auf „JPG Export“ erzeugt eine technische Zeichnung.
   - **Live-KI-Vorhersage:** Eingabefelder für Kommando & Distanz, Button für Sofort-Vorhersage.
   - **Simulationsmodus-Hinweis:** Gelber Banner oben im Dashboard.

4. **API-Endpunkte (Auszug):**
   - `POST /api/webcontrol/connect` – Verbindung (Simulationsmodus)
   - `POST /api/webcontrol/command` – Befehl senden
   - `POST /api/webcontrol/predict` – KI-Vorhersage (Kommando, Distanz)
   - `POST /api/webcontrol/emergency-stop` – Not-Aus
   - `POST /api/export` – JPG-Export

---

## 🧪 Testen

1. **Alle Tests ausführen:**
   - Im Test-Explorer von Visual Studio: „Alle Tests ausführen“
   - Oder per Konsole:
```sh
 dotnet test
```

2. **Testabdeckung:**
   - Tests für Simulation, API, Logger, Map, Export, Datenbank, KI-Vorhersage.

---

## 🤖 KI-Integration (ML.NET-ready)

- Die KI-Vorhersage ist generisch vorbereitet (`PredictionService`).
- Im Simulationsmodus werden Zufallswerte geliefert.
- **Echtes ML.NET-Modell einbinden:**  
  - Installiere das NuGet-Paket `Microsoft.ML`.
  - Ersetze die Logik in `PredictionService` durch das Laden und Anwenden deines Modells.
  - Die API und das Frontend müssen nicht angepasst werden.

---

## ⚠️ Hinweise

- **Simulationsmodus:** Standardmäßig aktiv, alle Daten sind virtuell. Umschalten auf echten Betrieb durch Anpassung des Parameters `simulate` in `RobotGateway`.
- **Plattform:** JPG-Export nutzt System.Drawing und ist für Windows optimiert (Warnung CA1416 unterdrückt).
- **Datenbank:** LocalDB oder SQL Server erforderlich.

---

## 🛡️ Sicherheit & Erweiterung

- **Benutzerverwaltung:** Kann bei Bedarf mit ASP.NET Identity/JWT ergänzt werden.
- **Deployment:** Dockerfile und Azure-Deployment können einfach ergänzt werden.
- **Dokumentation:** API-Doku (Swagger/OpenAPI) kann mit wenigen Zeilen aktiviert werden.

---

## 👨‍💻 Entwicklerhinweise

- **Simulationsdaten:** Werden automatisch generiert und in `learning_data.jsonl` gespeichert.
- **KI-Modelle:** Trainingsdaten können mit Python, scikit-learn oder ML.NET erzeugt werden.
- **Erweiterung:** Neue Features (z.B. Heatmaps, Anomalieerkennung, Benutzerverwaltung) können modular ergänzt werden.

---

## 📧 Kontakt

**Autor:** Marcus Reiser  
**GitHub:** [marcus39-web](https://github.com/marcus39-web)  
**Lizenz:** MIT License – frei verwendbar für Bildungs- und Demonstrationszwecke

---

**Viel Erfolg beim Testen**