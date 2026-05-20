using System.Security.Claims;
using EcommerceApp1.Data;
using EcommerceApp1.Models;
using EcommerceApp1.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;

namespace EcommerceApp1.Controllers;

public class ShoppingCartController : Controller
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;
    
    public ShoppingCartController(IShoppingCartRepository shoppingCartRepository, ApplicationDbContext context, EmailService emailService)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _context = context;
        _emailService = emailService;
    }

    public IActionResult GetCart()
    {
        var shoppingCart = _shoppingCartRepository.GetShoppingCart();
        var cartData = shoppingCart.CartItems
            .Join(
                _context.Products, 
                cartItem => cartItem.ProductId,
                product => product.ProductId, 
                (cartItem, product) => new { cartItem, product }
            )
            .Select(c => new { 
                CartItemId = c.product.ProductId, 
                CartItemName = c.product.Name, 
                CartItemQuantity = c.cartItem.Quantity,
                UnitPrice = c.product.Price,                              
                SubTotal = c.cartItem.Quantity * c.product.Price         
            })
            .ToList();

        var result = new {
            items = cartData,
            totalItems = cartData.Sum(c => c.CartItemQuantity),           
            totalPrice = cartData.Sum(c => c.SubTotal)                   
        };

        return Json(result);
    }

    public IActionResult AddCartItem(int productId, int quantity = 1)
    {
        _shoppingCartRepository.AddCartItem(productId, quantity);
        _shoppingCartRepository.SaveChanges();
        
        return Redirect("/Products/Index");
    }

    public IActionResult UpdateCartItemQuantity(int productId, int action)
    {
        _shoppingCartRepository.UpdateCartItemQuantity(productId, action);
        _shoppingCartRepository.SaveChanges();
        return Ok();
    }
    
    public IActionResult DeleteCartItem(int productId)
    {
        _shoppingCartRepository.DeleteCartItem(productId);
        _shoppingCartRepository.SaveChanges();
        return Ok();
    }
    
    public IActionResult Cart()
    {
        return View();
    }
    
    public IActionResult Checkout()
    {
        return View();
    }

        [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        // 1. Load cart with product details
        var shoppingCart = _shoppingCartRepository.GetShoppingCart();
        var cartData = shoppingCart.CartItems
            .Join(
                _context.Products,
                cartItem => cartItem.ProductId,
                product => product.ProductId,
                (cartItem, product) => new { cartItem, product }
            )
            .ToList();

        if (!cartData.Any())
            return BadRequest("Cart is empty");

        // 2. Create the order
        var order = new Models.Order
        {
            UserId     = _shoppingCartRepository.GetUserId(),
            OrderDate = DateTime.UtcNow,
            Status     = "Pending",
            FirstName  = request.FirstName,
            LastName   = request.LastName,
            Email      = request.Email,
            Address    = request.Address,
            City       = request.City,
            State      = request.State,
            Zip        = request.Zip,
            TotalPrice = cartData.Sum(c => c.cartItem.Quantity * (double)c.product.Price),
            OrderItems = cartData.Select(c => new OrderItem
            {
                ProductId = c.product.ProductId,
                Quantity  = c.cartItem.Quantity,
                UnitPrice     = (double)c.product.Price     // snapshot at purchase time
            }).ToList()
        };

        _context.Orders.Add(order);

        // 3. Clear the cart
        _context.CartItems.RemoveRange(shoppingCart.CartItems);

        await _context.SaveChangesAsync();

        // 4. Send invoice email
        var emailBody = BuildInvoiceEmail(order, cartData.Select(c => new
        {
            c.product.Name,
            c.cartItem.Quantity,
            c.product.Price,
            SubTotal = c.cartItem.Quantity * c.product.Price
        }).ToList<dynamic>());

        await _emailService.SendEmailAsync(
            fromName: "Merchandise Store",
            subject: $"Order Confirmation #{order.OrderId}",
            message: emailBody,
            toEmail: request.Email
        );

        return Ok(new { orderId = order.OrderId });
    }

    public IActionResult OrderConfirmation(int orderId)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.OrderId == orderId);

        if (order == null) return NotFound();

        return View(order);
    }

    private string BuildInvoiceEmail(Models.Order order, List<dynamic> items)
    {
        var rows = string.Join("", items.Select(item => $"""
            <tr>
                <td style="padding:12px 16px;border-bottom:1px solid #f0f0f0">{item.Name}</td>
                <td style="padding:12px 16px;border-bottom:1px solid #f0f0f0;text-align:center">{item.Quantity}</td>
                <td style="padding:12px 16px;border-bottom:1px solid #f0f0f0;text-align:right">${item.Price:F2}</td>
                <td style="padding:12px 16px;border-bottom:1px solid #f0f0f0;text-align:right">${item.SubTotal:F2}</td>
            </tr>
        """));

        return $"""
        <!DOCTYPE html>
        <html>
        <body style="margin:0;padding:0;background:#f6f6f6;font-family:'Helvetica Neue',Arial,sans-serif">
          <table width="100%" cellpadding="0" cellspacing="0" style="padding:40px 0">
            <tr><td align="center">
              <table width="600" cellpadding="0" cellspacing="0"
                     style="background:#ffffff;border-radius:8px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,.08)">

                <!-- Header -->
                <tr>
                  <td style="background:#1a1a1a;padding:32px 40px">
                    <h1 style="margin:0;color:#ffffff;font-size:24px;font-weight:700">Order Confirmed</h1>
                    <p style="margin:6px 0 0;color:#999;font-size:14px">Order #{order.OrderId} · {order.OrderDate:MMMM dd, yyyy}</p>
                  </td>
                </tr>

                <!-- Greeting -->
                <tr>
                  <td style="padding:32px 40px 16px">
                    <p style="margin:0;font-size:16px;color:#333">
                        Hi <strong>{order.FirstName}</strong>, thank you for your order!
                        We'll notify you when it ships.
                    </p>
                  </td>
                </tr>

                <!-- Items table -->
                <tr>
                  <td style="padding:0 40px 24px">
                    <table width="100%" cellpadding="0" cellspacing="0"
                           style="border:1px solid #f0f0f0;border-radius:6px;overflow:hidden">
                      <thead>
                        <tr style="background:#f9f9f9">
                          <th style="padding:12px 16px;text-align:left;font-size:12px;text-transform:uppercase;color:#999;font-weight:600">Item</th>
                          <th style="padding:12px 16px;text-align:center;font-size:12px;text-transform:uppercase;color:#999;font-weight:600">Qty</th>
                          <th style="padding:12px 16px;text-align:right;font-size:12px;text-transform:uppercase;color:#999;font-weight:600">Price</th>
                          <th style="padding:12px 16px;text-align:right;font-size:12px;text-transform:uppercase;color:#999;font-weight:600">Subtotal</th>
                        </tr>
                      </thead>
                      <tbody>
                        {rows}
                      </tbody>
                    </table>
                  </td>
                </tr>

                <!-- Total -->
                <tr>
                  <td style="padding:0 40px 32px">
                    <table width="100%" cellpadding="0" cellspacing="0">
                      <tr>
                        <td style="padding:4px 0;color:#999;font-size:14px">Shipping</td>
                        <td style="padding:4px 0;text-align:right;color:#22c55e;font-size:14px;font-weight:600">Free</td>
                      </tr>
                      <tr>
                        <td style="padding:12px 0 4px;font-size:18px;font-weight:700;color:#1a1a1a;border-top:2px solid #f0f0f0">Total</td>
                        <td style="padding:12px 0 4px;text-align:right;font-size:18px;font-weight:700;color:#1a1a1a;border-top:2px solid #f0f0f0">${order.TotalPrice:F2}</td>
                      </tr>
                    </table>
                  </td>
                </tr>

                <!-- Shipping address -->
                <tr>
                  <td style="padding:24px 40px;background:#f9f9f9;border-top:1px solid #f0f0f0">
                    <p style="margin:0 0 6px;font-size:12px;text-transform:uppercase;color:#999;font-weight:600">Shipping To</p>
                    <p style="margin:0;font-size:14px;color:#333;line-height:1.6">
                        {order.FirstName} {order.LastName}<br>
                        {order.Address}<br>
                        {order.City}, {order.State} {order.Zip}
                    </p>
                  </td>
                </tr>

                <!-- Footer -->
                <tr>
                  <td style="padding:24px 40px;text-align:center">
                    <p style="margin:0;font-size:12px;color:#bbb">
                        Questions? Reply to this email or contact support.
                    </p>
                  </td>
                </tr>

              </table>
            </td></tr>
          </table>
        </body>
        </html>
        """;
    }
}