using System.Text.RegularExpressions;

// начальные данные


List<MyDictionary> users = new List<MyDictionary>
{
    /*
    
    Вообще, по идее, данные ниже по плану должны браться из бд, и туда же сохраняться. Но я пока не владею этими знаниями. Из того что задумано сделанно только 10%.
    Также прложение должно давать выбор одного из 3х языков: английский , испанский и китайский. должна быть проверка знаний . Из бд достается рандомное слово и пользователь (я, так как это пэт проект для личного пользования) пишет прервод , 
    тапает на кнопку проверки и выдает сообщение правда/ложь. Так же  пользователь должен будет пройти авторизацию и соответсвенно у каждого пользователя свой словарь.
    Пока не могу даже пред положить сколько займет времени получение необходимых для этого знаний и написание самого приложения. 
    Честно было взято простейшее Api из примера сайта  metanit на котором и черпаю знания, и переделано под свои нужды.
      
    */



    // изменяем данные

    new() { Id = Guid.NewGuid().ToString(), Word = "One", Translate = "Один" },
    new() { Id = Guid.NewGuid().ToString(), Word = "Two", Translate = "Два" },
    new() { Id = Guid.NewGuid().ToString(), Word = "Three", Translate = "Три" },
    new() { Id = Guid.NewGuid().ToString(), Word ="Four", Translate ="Четыре"},
    new() { Id = Guid.NewGuid().ToString(), Word ="Hola", Translate ="Привет"},
    new() { Id = Guid.NewGuid().ToString(), Word ="你好！", Translate ="Привет"},
    new() { Id = Guid.NewGuid().ToString(), Word ="Obtain", Translate ="Получать"},
    new() { Id = Guid.NewGuid().ToString(), Word ="tracing", Translate ="отследивание, трассировка"},
    new() { Id = Guid.NewGuid().ToString(), Word ="execution", Translate ="исполнение"},
    new() { Id = Guid.NewGuid().ToString(), Word ="contains", Translate ="содержит"},
    new() { Id = Guid.NewGuid().ToString(), Word ="Dictionary", Translate ="словарь"}







};

var builder = WebApplication.CreateBuilder();
var app = builder.Build();




app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;
    //string expressionForNumber = "^/api/users/([0 - 9]+)$";   // если id представляет число

    // 2e752824-1657-4c7f-844b-6ec2e168e99c
    string expressionForGuid = @"^/api/translates/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
    if (path == "/api/translates" && request.Method == "GET")
    {
        await GetAllWord(response);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
    {
        // получаем id из адреса url
        string? id = path.Value?.Split("/")[3];
        await GetMyDictionary(id, response, request);
    }
    else if (path == "/api/translates" && request.Method == "POST")
    {
        await CreateMyDictionary(response, request);
    }
    else if (path == "/api/translates" && request.Method == "PUT")
    {
        await UpdateMyDictionary(response, request);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
    {
        string? id = path.Value?.Split("/")[3];
        await DeleteMyDictionary(id, response, request);
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();

// получение все слова перевода
async Task GetAllWord(HttpResponse response)
{
    await response.WriteAsJsonAsync(users);
}
// получение один перевод по id
async Task GetMyDictionary(string? id, HttpResponse response, HttpRequest request)
{
    // получаем перевод по id
    MyDictionary? user = users.FirstOrDefault((u) => u.Id == id);
    // если перевод найден, отправляем его
    if (user != null)
        await response.WriteAsJsonAsync(user);
    // если не найден, отправляем статусный код и сообщение об ошибке
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Перевод не найден" });
    }
}

async Task DeleteMyDictionary(string? id, HttpResponse response, HttpRequest request)
{
    // получаем перевод по id
    MyDictionary? user = users.FirstOrDefault((u) => u.Id == id);
    // если перевод найден, удаляем его
    if (user != null)
    {
        users.Remove(user);
        await response.WriteAsJsonAsync(user);
    }
    // если не найден, отправляем статусный код и сообщение об ошибке
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "Перевод не найден" });
    }
}

async Task CreateMyDictionary(HttpResponse response, HttpRequest request)
{
    try
    {
        // получаем данные перевода
        var user = await request.ReadFromJsonAsync<MyDictionary>();
        if (user != null)
        {
            // устанавливаем id для нового перевод
            user.Id = Guid.NewGuid().ToString();
            // добавляем перевод в список
            users.Add(user);
            await response.WriteAsJsonAsync(user);
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
}

async Task UpdateMyDictionary(HttpResponse response, HttpRequest request)


{
    try
    {
        // получаем перевод пользователя
        MyDictionary? userData = await request.ReadFromJsonAsync<MyDictionary>();
        if (userData != null)
        {
            // получаем перевод по id
            var user = users.FirstOrDefault(u => u.Id == userData.Id);
            // если перевод найден, изменяем его данные и отправляем обратно клиенту
            if (user != null)
            {
            
                user.Word = userData.Word;
                user.Translate = userData.Translate;
                await response.WriteAsJsonAsync(user);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "Перевод не найден" });
            }
        }
        else
        {
            throw new Exception("Некорректные данные");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
    }
}


//[Serializable]
//[System.Runtime.Serialization.DataContract]
public class MyDictionary
{
    public string Id { get; set; } = "";
    public string Word { get; set; } = "";
    public string? Translate { get; set; } = "";
}

