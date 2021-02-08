<div align="center">
<h1>MyFaceApi</h1>
</div>

Projekt jest RESTowym API dla portalu społecznościowego [Myface](https://github.com/JacDev/MyFaceClient). 
Zawiera także IdentityServer do autentykacji i autoryzacji użytkowników oraz autoryzacji zapytań do API.

API obsługuje:
- pobieranie, dodawanie, edytowanie oraz usuwanie:
  - postów;
  - komentarzy do postów;
  - reakcji do postów;
  - relacji znajomości;
  - powiadomień;
  - wiadomości;
- pobieranie użytkowników z IdentityServera.

IdentytyServer obsługuje:
- rejestrowanie oraz logowanie użytkowników;
- autoryzowanie zapytań do API.

Projekt ten wykorzystuje także SignalR do obsługi powiadomień oraz czatu na żywo.

Po przekierowaniu na stonę IdentityServera użytkownikowi pokazywane jest okno dialogowe informujące o tym, że jest to projekt demo (wszystkie screeny pokazują wyświetlanie na dużym i małym ekranie):
<br>

![Image of Yaktocat](https://github.com/JacDev/MyFaceApi/blob/master/Readme/Images/FirstPage.png)

W przypadku pomyślnego zalogowania użytkownik jest przekierowany na stronę, z której przybył. Jeśli login lub hasło wprowadzone przez użytkownika są nieprawidłowe, pojawia się o tym informacja:
<br>

![Image of Yaktocat](https://github.com/JacDev/MyFaceApi/blob/master/Readme/Images/LoginIncorrectData.png)

Po naciśnięciu przycisku "Utwórz konto" użytkownik jest przekierowywany na stronę rejestracji:
<br>

![Image of Yaktocat](https://github.com/JacDev/MyFaceApi/blob/master/Readme/Images/RegisterPage.png)

W przypadku niewypełnienia któregoś z wymaganych pól lub w sytuacji, gdy podany login jest już zajęty, wyświetlany jest komunikat:
<br>

![Image of Yaktocat](https://github.com/JacDev/MyFaceApi/blob/master/Readme/Images/RegisterIncorrectData.png) 

![Image of Yaktocat](https://github.com/JacDev/MyFaceApi/blob/master/Readme/Images/LoginTaken.png)

