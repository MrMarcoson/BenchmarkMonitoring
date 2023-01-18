# BenchmarkMonitoring

Program monitorujący statystyki systemu.

- Usługa czytająca dane z systemu - statystyki CPU, GPU i RAM i zapisująca je do plików .json. Zapisuje do dziennika zdarzeń informację, gdy odczytuje informacje z systemu. Serwisowi podaje się w app.config: gdzie ma zapisywać pliki, co jaki czas ma to robić, nazwę i źródło logera.

- Aplikacja konsolowa deserializująca jsony i wyświetlająca dane dla użytkownika.

- Biblioteka implementuje zbieranie danych systemowych, poprzez WMI i OpenHardwareMonitorLib.

- Serwis posiada instalator.

- Biblioteka zawiera testy sprawdzające: czy zgadzają się monitorowane urządzenia, poprawność temperatur gpu, poprawność ilości rdzeni w procesorze i deserializację jsonów.

![obraz](https://user-images.githubusercontent.com/67783947/213196113-0a9f3cda-f8fc-41f3-b20e-8049fbe6c2fe.png)
![obraz](https://user-images.githubusercontent.com/67783947/213196189-bdce406f-b584-432d-890e-8dbe5b011738.png)
![obraz](https://user-images.githubusercontent.com/67783947/213196299-ac465c23-4d7e-44af-8a27-128642852578.png)

