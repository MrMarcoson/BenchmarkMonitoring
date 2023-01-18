# BenchmarkMonitoring

Program monitorujący statystyki systemu.

- Usługa czytająca dane z systemu - statystyki CPU, GPU i RAM i zapisująca je do plików .json. Zapisuje do dziennika zdarzeń informację, gdy odczytuje informacje z systemu. Serwisowi podaje się w app.config: gdzie ma zapisywać pliki, co jaki czas ma to robić, nazwę i źródło logera.

- Aplikacja konsolowa deserializująca jsony i wyświetlająca dane dla użytkownika.

- Biblioteka implementuje zbieranie danych systemowych, poprzez WMI i OpenHardwareMonitorLib.

- Serwis posiada instalator.

- Biblioteka zawiera testy sprawdzające: czy zgadzają się monitorowane urządzenia, poprawność temperatur gpu, poprawność ilości rdzeni w procesorze i deserializację jsonów.


