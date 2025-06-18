// Sepet verilerini localStorage'dan al
let cart = JSON.parse(localStorage.getItem('cart')) || [];

// Sayfa yüklendiğinde sepeti göster
document.addEventListener('DOMContentLoaded', () => {
    displayCart();
    updateCartCount();
});

// Sepeti görüntüle
function displayCart() {
    const cartItemsContainer = document.getElementById('cartItems');
    const subtotalElement = document.getElementById('subtotal');
    const taxElement = document.getElementById('tax');
    const totalElement = document.getElementById('total');

    if (cart.length === 0) {
        cartItemsContainer.innerHTML = `
            <div class="col-12 text-center">
                <p class="lead">Sepetiniz boş</p>
                <a href="/Home/Products" class="btn btn-primary">Alışverişe Başla</a>
            </div>
        `;
        updateTotals(0);
        return;
    }

    cartItemsContainer.innerHTML = cart.map(item => `
        <div class="col-12 mb-3">
            <div class="card">
                <div class="card-body">
                    <div class="row align-items-center">
                        <div class="col-md-2">
                            <img src="${item.imageUrl || 'https://placehold.co/400x300?text=Enstrüman'}" 
                                 alt="${item.title}" 
                                 class="img-fluid rounded"
                                 onerror="this.src='https://placehold.co/400x300?text=Enstrüman'">
                        </div>
                        <div class="col-md-4">
                            <h5 class="card-title">${item.title}</h5>
                            <p class="card-text text-muted">${item.brand}</p>
                        </div>
                        <div class="col-md-2">
                            <div class="input-group">
                                <button class="btn btn-outline-secondary" onclick="updateQuantity(${item.id}, -1)">-</button>
                                <input type="number" class="form-control text-center" value="${item.quantity}" readonly>
                                <button class="btn btn-outline-secondary" onclick="updateQuantity(${item.id}, 1)">+</button>
                            </div>
                        </div>
                        <div class="col-md-2 text-end">
                            <p class="card-text">${(item.price * item.quantity).toLocaleString('tr-TR')} TL</p>
                        </div>
                        <div class="col-md-2 text-end">
                            <button class="btn btn-danger" onclick="removeFromCart(${item.id})">
                                <i class="bi bi-trash"></i> Kaldır
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `).join('');

    // Toplam tutarları güncelle
    const subtotal = cart.reduce((total, item) => total + (item.price * item.quantity), 0);
    updateTotals(subtotal);
}

// Miktar güncelle
function updateQuantity(productId, change) {
    const itemIndex = cart.findIndex(item => item.id === productId);
    if (itemIndex > -1) {
        cart[itemIndex].quantity = Math.max(1, cart[itemIndex].quantity + change);
        localStorage.setItem('cart', JSON.stringify(cart));
        displayCart();
        updateCartCount();
    }
}

// Sepetten ürün kaldır
function removeFromCart(productId) {
    cart = cart.filter(item => item.id !== productId);
    localStorage.setItem('cart', JSON.stringify(cart));
    displayCart();
    updateCartCount();
}

// Toplam tutarları güncelle
function updateTotals(subtotal) {
    const tax = subtotal * 0.18;
    const total = subtotal + tax;

    document.getElementById('subtotal').textContent = `${subtotal.toLocaleString('tr-TR')} TL`;
    document.getElementById('tax').textContent = `${tax.toLocaleString('tr-TR')} TL`;
    document.getElementById('total').textContent = `${total.toLocaleString('tr-TR')} TL`;
}

// Sepet sayısını güncelle
function updateCartCount() {
    const cartCount = document.getElementById('cartCount');
    const totalItems = cart.reduce((total, item) => total + item.quantity, 0);
    cartCount.textContent = totalItems;
}

// Siparişi tamamla
function checkout() {
    if (cart.length === 0) {
        alert('Sepetiniz boş!');
        return;
    }

    // Burada sipariş tamamlama işlemleri yapılacak
    alert('Sipariş tamamlama özelliği yakında eklenecek!');
} 