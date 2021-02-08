# MyFaceApi
Projekt jest RESTowym Api dla portalu społecznościowego [Myface](https://github.com/JacDev/MyFaceClient)

Api obsługuje:
- pobieranie, dodawanie, edytowanie oraz usuwanie:
  - postów;
  - komentarzy do postów;
  - reakcji do postów;
  - relacji znajomości;
  - powiadomień;
  - wiadomości.

Projekt ten wykorzystuje także SignalR do obsługi powiadomień oraz czatu na żywo.
Dostęp do metod api wymaga autoryzacji, która dokonywana jest przy pomocy IdentityServera dostępnego w tym rozwiązaniu.
