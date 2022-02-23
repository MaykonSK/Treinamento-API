using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var configuration = app.Configuration;
ProductData.Init(configuration);

app.MapGet("/", () => "Hello World!");
app.MapGet("/p1", () => "Hello Word 2!");

//passando dados pelo parametro
app.MapGet("/getproduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) => {
    return dateStart +" - "+ dateEnd;
});
// https://localhost:7219/getproduct?datestart=x&dateend=y

//Requisição
app.MapGet("/getproductheader", (HttpRequest request) => {
    return request.Headers["product-code"].ToString();
});

// *************CRUD*****************

app.MapPost("/products", (Product product) => {
    ProductData.AddProduto(product);
    //status code
    return Results.Created("/products/"+product.code, product.code);
});

app.MapGet("/products/{code}", ([FromRoute] int code) => {
    // return code;
    var product = ProductData.getProduct(code);
    if (product != null) {
        return Results.Ok(product);
    } else {
        return Results.NotFound();
    }
});

app.MapPut("/products", (Product product) => {
    //obtem o produto
    var productSaved = ProductData.getProduct(product.code);
    //edita produto
    productSaved.name = product.name;
    return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] int code) => {
    //obter produto primeiro
    var productSaved = ProductData.getProduct(code);
    ProductData.RemoverProduto(productSaved);
    return Results.Ok();
});

//retorna configuraçao api
app.MapGet("/database", (IConfiguration configuration) => {
    return configuration["ConnectionStrings:conexaoAPI"];
});

app.Run();

//List tem a mesma função do array, mas com buscas mais avançadas

public static class ProductData {
    public static List<Product> Products { get; set; } = Products = new List<Product>(); //lista da classe de product

    public static void Init(IConfiguration configuration) {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    //adiciona
    public static void AddProduto(Product product) {
        //adiciona item na lista
        Products.Add(product);
    }

    //recupera
    public static Product getProduct(int code) {
        return Products.FirstOrDefault(p => p.code == code); //expressão lambda, First obtem o primeiro
    }

    //remove
    public static void RemoverProduto(Product product) {
        Products.Remove(product);
    }
}

public class Product {
    public int code { get; set; }
    public string name { get; set; }
}