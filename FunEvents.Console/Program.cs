using System.Net.Http.Json;
using System.Text.Json;

const string DefaultApiUrl = "http://localhost:5144";

var apiUrl = Environment.GetEnvironmentVariable("FUNEVENTS_API_URL") ?? DefaultApiUrl;

using var http = new HttpClient { BaseAddress = new Uri(apiUrl) };

int op;
do
{

    Console.WriteLine("\n\n\t\t\t\t ********** FunEvents ********** \t\t\t\t\n\n");
    Console.Write("\t\t1<---------- Reservar\n\n\t\t2<---------- Salir\n\n\t\tDigite una opcion: ");

    op = int.Parse(Console.ReadLine()!);


    switch (op)
    {
        case 1:
            
            Console.WriteLine("\n\n\t\t\t\t ********** FunEvents ********** \t\t\t\t\n\n");

            Console.WriteLine(" Reservas\n");
            var seededUserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");
            var seededEventCode = "X2026X24X12";

            Console.Write("Digite el Id del usuario: ");
            string idUser = Console.ReadLine()!;

            Console.WriteLine();

            Console.Write("Digite el codigo del evento: ");
            string code = Console.ReadLine()!;

            Console.WriteLine();

            Console.Write("cuantas entradas desea adquirir ? maximo 10: ");

            int cantidad = int.Parse(Console.ReadLine()!);

            var reservationRequest = new
            {
                eventCode = code,
                userId = Guid.Parse(idUser),
                quantity = cantidad
            };

            Console.WriteLine($"Intentando reservar {cantidad} entradas para '{seededEventCode}' como usuario {seededUserId} en {apiUrl}");

            await Create(reservationRequest);

            break;
        case 2:
            Console.WriteLine("Sesión finalizada...");
            break;
        default:
            break;
    }

   

} while ( op!=2);


async Task Create(object reservationRequest)
{
    try
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        var content = JsonContent.Create(reservationRequest, options: options);

        using var response = await http.PostAsync("/api/reservations", content);

        var body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Reserva creada. Estado: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.WriteLine("Respuesta:");
            Console.WriteLine(body);
        }
        else
        {
            Console.WriteLine($"Error al crear la reserva. Estado: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.WriteLine("Contenido:");
            Console.WriteLine(body);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error durante la petición HTTP:");
        Console.WriteLine(ex.ToString());
    }
}


