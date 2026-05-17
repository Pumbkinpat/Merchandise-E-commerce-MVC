using System.Security.Claims;
using EcommerceApp1.Data;
using EcommerceApp1.Models;
using EcommerceApp1.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;

namespace EcommerceApp1.Controllers;

public class ShoppingCartController : Controller
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ApplicationDbContext _context;
    
    public class ShoppingCart 
    {
        public string CartItemName;
        public int CartItemQuantity;
    }

    public class ShoppingCartDisplayModel
    {
        public List<ShoppingCart> data;
    };
    
    public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, ApplicationDbContext context)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _context = context;
    }

    public IActionResult GetCart()
    {
        var shoppingCart = _shoppingCartRepository.GetShoppingCart();
        var shoppingCartDisplayModel = shoppingCart.CartItems
            .Join(
                _context.Products, 
                cartItem => cartItem.ProductId,
                product => product.ProductId, 
                (cartItem, product) => new { cartItem, product }
            )
            .Select(c => new { CartItemId = c.product.ProductId, CartItemName = c.product.Name, CartItemQuantity = c.cartItem.Quantity})
            .ToList();

        ViewBag.ShoppingCart = shoppingCartDisplayModel;

        return Json(shoppingCartDisplayModel);
    }

    public IActionResult AddCartItem(int productId, int quantity = 1)
    {
        _shoppingCartRepository.AddCartItem(productId, quantity);
        _shoppingCartRepository.SaveChanges();
        
        return Redirect("/Products/Index");
    }

    public void UpdateCartItemQuantity(int productId, int action)
    {
        _shoppingCartRepository.UpdateCartItemQuantity(productId, action);
        _shoppingCartRepository.SaveChanges();
    }
}