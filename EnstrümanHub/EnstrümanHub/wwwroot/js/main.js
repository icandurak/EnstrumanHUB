// API endpoint'leri
const API_BASE_URL = '/api';

// Tüm enstrümanları getir
async function getAllInstruments() {
    try {
        console.log('Enstrümanlar getiriliyor...');
        const [guitars, bassGuitars, drums] = await Promise.all([
            fetch(`${API_BASE_URL}/Enstruman/Gitar`).then(res => {
                console.log('Gitarlar yanıtı:', res.status);
                return res.json();
            }),
            fetch(`${API_BASE_URL}/Enstruman/Bass`).then(res => {
                console.log('Bas gitarlar yanıtı:', res.status);
                return res.json();
            }),
            fetch(`${API_BASE_URL}/Enstruman/Bateri`).then(res => {
                console.log('Bateriler yanıtı:', res.status);
                return res.json();
            })
        ]);

        console.log('Gitarlar:', guitars);
        console.log('Bas Gitarlar:', bassGuitars);
        console.log('Bateriler:', drums);

        // API'den gelen verileri düzenle
        const formattedGuitars = guitars.map(g => ({
            id: g.id,
            title: g.title,
            brand: g.brand,
            price: g.price,
            imageUrl: g.imageUrl,
            description: g.description,
            category: 'gitar'
        }));

        const formattedBassGuitars = bassGuitars.map(b => ({
            id: b.id,
            title: b.title,
            brand: b.brand,
            price: b.price,
            imageUrl: b.imageUrl,
            description: b.description,
            category: 'bass'
        }));

        const formattedDrums = drums.map(d => ({
            id: d.id,
            title: d.title,
            brand: d.brand,
            price: d.price,
            imageUrl: d.imageUrl,
            description: d.description,
            category: 'bateri'
        }));

        return [...formattedGuitars, ...formattedBassGuitars, ...formattedDrums];
    } catch (error) {
        console.error('Enstrümanlar yüklenirken hata oluştu:', error);
        return [];
    }
}

// Tüm ürünleri saklamak için global değişken
let allProducts = [];

// Ürünleri filtrele
function filterProducts(category) {
    // Aktif kategori butonunu güncelle
    document.querySelectorAll('.list-group-item').forEach(btn => {
        btn.classList.remove('active');
        if (btn.dataset.category === category) {
            btn.classList.add('active');
        }
    });

    // Ürünleri filtrele
    const filteredProducts = category === 'all' 
        ? allProducts 
        : allProducts.filter(product => product.category === category);

    // Filtrelenmiş ürünleri göster
    displayProducts(filteredProducts);
}

// Ürünleri sırala
function sortProducts(sortType) {
    const products = [...allProducts];
    
    switch (sortType) {
        case 'price-asc':
            products.sort((a, b) => a.price - b.price);
            break;
        case 'price-desc':
            products.sort((a, b) => b.price - a.price);
            break;
    }

    displayProducts(products);
}

// Ürünleri görüntüle
function displayProducts(products) {
    const productsContainer = document.getElementById('productsGrid');
    if (!productsContainer) {
        console.log('Ürünler container bulunamadı');
        return;
    }

    if (products.length === 0) {
        productsContainer.innerHTML = '<div class="col-12"><p class="text-center">Bu kategoride ürün bulunmamaktadır.</p></div>';
        return;
    }

    productsContainer.innerHTML = products.map(product => createProductCard(product)).join('');
}

// Kategorileri getir
async function getCategories() {
    try {
        console.log('Kategoriler getiriliyor...');
        const response = await fetch(`${API_BASE_URL}/Categories`);
        console.log('Kategoriler API yanıtı:', response.status);
        const categories = await response.json();
        console.log('Kategoriler:', categories);
        return categories;
    } catch (error) {
        console.error('Kategoriler yüklenirken hata oluştu:', error);
        return [];
    }
}

// Kategori menüsünü oluştur
function createCategoryMenu(categories) {
    const categoryMenu = document.querySelector('.list-group');
    if (!categoryMenu) return;

    // Tüm ürünler butonu
    const allProductsButton = `
        <button type="button" 
                class="list-group-item list-group-item-action active" 
                data-category="all"
                onclick="filterProducts('all')">
            Tüm Ürünler
        </button>
    `;

    // Kategori butonları
    const categoryButtons = categories.map(category => `
        <button type="button" 
                class="list-group-item list-group-item-action" 
                data-category="${category.name.toLowerCase()}"
                onclick="filterProducts('${category.name.toLowerCase()}')">
            ${category.name}
        </button>
    `).join('');

    categoryMenu.innerHTML = allProductsButton + categoryButtons;
}

// Sepete ürün ekle
async function addToCart(productId) {
    try {
        // Önce gitar olarak dene
        let response = await fetch(`${API_BASE_URL}/Enstruman/Gitar/${productId}`);
        if (!response.ok) {
            // Sonra bas gitar olarak dene
            response = await fetch(`${API_BASE_URL}/Enstruman/Bass/${productId}`);
            if (!response.ok) {
                // Son olarak bateri olarak dene
                response = await fetch(`${API_BASE_URL}/Enstruman/Bateri/${productId}`);
                if (!response.ok) {
                    throw new Error('Ürün bulunamadı');
                }
            }
        }

        const product = await response.json();
        
        let cart = JSON.parse(localStorage.getItem('cart')) || [];
        const existingItem = cart.find(item => item.id === productId);

        if (existingItem) {
            existingItem.quantity += 1;
        } else {
            cart.push({
                id: product.id,
                title: product.title,
                brand: product.brand,
                price: product.price,
                imageUrl: product.imageUrl,
                quantity: 1
            });
        }

        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartCount();
        alert('Ürün sepete eklendi!');
    } catch (error) {
        console.error('Sepete eklenirken hata oluştu:', error);
        alert('Ürün sepete eklenirken bir hata oluştu!');
    }
}

// Sepet sayısını güncelle
function updateCartCount() {
    const cartCount = document.getElementById('cartCount');
    if (!cartCount) return;
    
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    const totalItems = cart.reduce((total, item) => total + item.quantity, 0);
    cartCount.textContent = totalItems;
}

// Ürün detaylarını göster
function showDetails(productId) {
    window.location.href = `/Home/ProductDetails/${productId}`;
}

// Ürün kartı oluştur
function createProductCard(product) {
    return `
        <div class="product-card">
            <div class="product-image">
                <img src="${product.imageUrl || 'https://placehold.co/400x300?text=Enstrüman'}" 
                     alt="${product.title}" 
                     onerror="this.src='https://placehold.co/400x300?text=Enstrüman'">
                ${product.stock <= 0 ? '<div class="product-badge" style="background: #dc3545;">Stokta Yok</div>' : ''}
            </div>
            <div class="product-info">
                <div class="product-brand">${product.brand}</div>
                <h3 class="product-title">${product.title}</h3>
                <div class="product-price">
                    <span class="current-price">${product.price.toLocaleString('tr-TR')} TL</span>
                </div>
                <div class="product-actions">
                    <button class="btn-add-cart" 
                            onclick="addToCart(${product.id})" 
                            ${product.stock <= 0 ? 'disabled' : ''}>
                        <i class="fas fa-shopping-cart"></i>
                        ${product.stock <= 0 ? 'Stokta Yok' : 'Sepete Ekle'}
                    </button>
                    <button class="btn-details" onclick="showDetails(${product.id})">
                        <i class="fas fa-info-circle"></i>
                        Detaylar
                    </button>
                </div>
            </div>
        </div>
    `;
}

// Sayfa yüklendiğinde
document.addEventListener('DOMContentLoaded', async () => {
    console.log('Sayfa yüklendi, veriler getiriliyor...');
    const productsContainer = document.getElementById('productsGrid');
    
    try {
        // Kategorileri yükle
        const categories = await getCategories();
        createCategoryMenu(categories);

        // Ürünleri yükle
        if (productsContainer) {
            console.log('Ürünler container bulundu, ürünler yükleniyor...');
            allProducts = await getAllInstruments();
            console.log('Yüklenen ürünler:', allProducts);
            
            if (allProducts && allProducts.length > 0) {
                displayProducts(allProducts);
            } else {
                productsContainer.innerHTML = '<div class="col-12"><p class="text-center">Henüz ürün bulunmamaktadır.</p></div>';
            }
        } else {
            console.log('Ürünler container bulunamadı');
        }
    } catch (error) {
        console.error('Veriler yüklenirken hata oluştu:', error);
        if (productsContainer) {
            productsContainer.innerHTML = '<div class="col-12"><p class="text-center text-danger">Veriler yüklenirken bir hata oluştu.</p></div>';
        }
    }
    
    updateCartCount();
}); 