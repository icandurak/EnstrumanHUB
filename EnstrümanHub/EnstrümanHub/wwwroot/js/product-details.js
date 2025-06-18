// Sayfa yüklendiğinde
document.addEventListener('DOMContentLoaded', async () => {
    // URL'den ürün ID'sini al
    const pathParts = window.location.pathname.split('/');
    const productId = pathParts[pathParts.length - 1];

    if (!productId) {
        showError('Ürün ID bulunamadı!');
        return;
    }

    try {
        // Önce gitar olarak dene
        let response = await fetch(`/api/Enstruman/Gitar/${productId}`);
        if (!response.ok) {
            // Sonra bas gitar olarak dene
            response = await fetch(`/api/Enstruman/Bass/${productId}`);
            if (!response.ok) {
                // Son olarak bateri olarak dene
                response = await fetch(`/api/Enstruman/Bateri/${productId}`);
                if (!response.ok) {
                    throw new Error('Ürün bulunamadı!');
                }
            }
        }

        const product = await response.json();
        displayProductDetails(product);
    } catch (error) {
        console.error('Ürün detayları yüklenirken hata oluştu:', error);
        showError('Ürün detayları yüklenirken bir hata oluştu!');
    }
});

// Ürün detaylarını göster
function displayProductDetails(product) {
    const productDetailsContainer = document.getElementById('productDetails');
    
    productDetailsContainer.innerHTML = `
        <div class="col-md-6">
            <img src="${product.imageUrl || 'https://placehold.co/600x400?text=Enstrüman'}" 
                 alt="${product.title}" 
                 class="img-fluid rounded"
                 onerror="this.src='https://placehold.co/600x400?text=Enstrüman'">
        </div>
        <div class="col-md-6">
            <h1 class="mb-3">${product.title}</h1>
            <p class="text-muted mb-2">Marka: ${product.brand}</p>
            <h3 class="text-primary mb-4">${product.price.toLocaleString('tr-TR')} TL</h3>
            <div class="mb-4">
                <h5>Ürün Açıklaması</h5>
                <p>${product.description || 'Ürün açıklaması bulunmamaktadır.'}</p>
            </div>
            <div class="mb-4">
                <h5>Özellikler</h5>
                <ul class="list-unstyled">
                    ${product.features ? product.features.map(feature => `
                        <li><i class="bi bi-check-circle-fill text-success me-2"></i>${feature}</li>
                    `).join('') : '<li>Özellik bilgisi bulunmamaktadır.</li>'}
                </ul>
            </div>
            <div class="d-grid gap-2">
                <button onclick="addToCart(${product.id})" class="btn btn-primary btn-lg">
                    Sepete Ekle
                </button>
                <a href="/Home/Products" class="btn btn-outline-secondary">
                    Ürünlere Dön
                </a>
            </div>
        </div>
    `;
}

// Hata mesajı göster
function showError(message) {
    const productDetailsContainer = document.getElementById('productDetails');
    productDetailsContainer.innerHTML = `
        <div class="col-12 text-center">
            <div class="alert alert-danger" role="alert">
                ${message}
            </div>
            <a href="/Home/Products" class="btn btn-primary">
                Ürünlere Dön
            </a>
        </div>
    `;
} 