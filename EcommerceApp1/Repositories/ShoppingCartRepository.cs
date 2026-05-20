using System.Security.Claims;
using EcommerceApp1.Data;
using EcommerceApp1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp1.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductsRepository _productsRepository;
    
    public ShoppingCartRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor, IProductsRepository productsRepository)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _productsRepository = productsRepository;
    }

    public string GetUserId()
    {
        ClaimsPrincipal currentUser = _httpContextAccessor.HttpContext.User;
        if (currentUser == null) throw new Exception("User not logged in");
        
        return _userManager.GetUserId(currentUser);
    }

    public ShoppingCart GetShoppingCart() /* Get or create a new shopping cart */
    {
        var userId = GetUserId();
        var shoppingCart = _context.ShoppingCarts
            .Include(sc => sc.CartItems)
            .FirstOrDefault(u => u.UserId == userId);

        if (shoppingCart == null)
        {
            shoppingCart = new ShoppingCart
            {
                UserId = GetUserId(),
                IsDeleted = false
            };

            _context.ShoppingCarts.Add(shoppingCart);
            SaveChanges();
        }

        shoppingCart = _context.ShoppingCarts
            .Include(sc => sc.CartItems) // Tell EF Core to load related entities to the shopping cart entity
            .First(u => u.UserId == userId);
        
        Console.WriteLine($"======================\n" +
                          $"Shopping Cart: {shoppingCart.CartItems}\n" +
                          $"======================");

        return shoppingCart;
    }

    public void AddCartItem(int productId, int quantity = 1)
    {
        var product = _productsRepository.GetById(productId);
        var userId = GetUserId();
        var shoppingCartList = _context.ShoppingCarts.Where(u => u.UserId == userId);
        var shoppingCart = GetShoppingCart();

        if (!shoppingCartList.Contains(shoppingCart)) throw new Exception("shopping cart not match");

        var existingCartItem = _context.CartItems.FirstOrDefault(c =>
            c.ShoppingCartId == shoppingCart.ShoppingCartId && c.ProductId == product.ProductId);

        Console.WriteLine($"======================\n" +
                          $"Add Cart Item:  existingItem={existingCartItem} - name={product.Name} - id={product.ProductId}\n" +
                          $"======================");
        
        if (existingCartItem != null)
        {
            existingCartItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                ShoppingCartId = shoppingCart.ShoppingCartId,
                ProductId = product.ProductId,
                Quantity = quantity
            };
            _context.CartItems.Add(cartItem);
        }
    }

    public void UpdateCartItemQuantity(int productId, int action)
    {
        var userId = GetUserId();
        
        var shoppingCart = _context.ShoppingCarts
            .FirstOrDefault(u => u.UserId == userId);

        if (shoppingCart == null) throw new Exception("Shopping cart not found");

        var product = _productsRepository.GetById(productId);

        var cartItem = _context.CartItems.FirstOrDefault(c =>
            c.ShoppingCartId == shoppingCart.ShoppingCartId && c.ProductId == product.ProductId);

        if (cartItem == null)
        {
            throw new Exception("No item in cart");
        }
        else if (cartItem.Quantity + action >= 1)
        {
            cartItem.Quantity += action;
        }
        else
        {
            _context.CartItems.Remove(cartItem);
        }
    }

    public void DeleteCartItem(int productId)
    {
        var product = _productsRepository.GetById(productId);
        var userId = GetUserId();
        var shoppingCartList = _context.ShoppingCarts.Where(u => u.UserId == userId);
        var shoppingCart = GetShoppingCart();

        if (!shoppingCartList.Contains(shoppingCart)) throw new Exception("shopping cart not match");

        var cartItem = _context.CartItems.FirstOrDefault(c =>
            c.ShoppingCartId == shoppingCart.ShoppingCartId && c.ProductId == product.ProductId);

        if (cartItem == null) throw new Exception("No cart item found");

        _context.CartItems.Remove(cartItem);
    }
    
    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}