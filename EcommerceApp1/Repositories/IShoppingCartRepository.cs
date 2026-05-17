using EcommerceApp1.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp1.Repositories;

public interface IShoppingCartRepository
{
    string GetUserId();
    ShoppingCart GetShoppingCart();
    void AddCartItem(int productId, int quantity = 1);
    void UpdateCartItemQuantity(int productId,int action);
    void DeleteCartItem(int productId);
    void SaveChanges();
}