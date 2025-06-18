// Products specific functionality
let currentFilters = {
    searchTerm: '',
    category: '',
    brand: '',
    minPrice: null,
    maxPrice: null,
    inStock: '',
    sortField: 'name',
    sortDirection: 'asc',
    page: 1,
    pageSize: 6
};

// Sample products data (will be replaced with API calls)
const sampleProducts = [
    {
        id: 1,
        name: "Fender Stratocaster Elektro Gitar",
        category: "gitar",
        brand: "fender",
        price: 15000,
        originalPrice: 18000,
        imageUrl: "https://images.unsplash.com/photo-1564186763535-ebb21ef5277f?w=280&h=250&fit=crop",
        rating: 4.5,
        reviewCount: 24,
        stock: 5,
        badge: "20% İndirim",
        description: "Profesyonel kalitede elektro gitar"
    },
    {
        id: 2,
        name: "Yamaha P-125 Dijital Piyano",
        category: "piyano",
        brand: "yamaha",
        price: 25000,
        originalPrice: null,
        imageUrl: "https://images.unsplash.com/photo-1520523839897-bd0b52f945a0?w=280&h=250&fit=crop",
        rating: 4.8,
        reviewCount: 18,
        stock: 3,
        badge: null,
        description: "88 tuşlu ağırlıklı dijital piyano"
    },
    // Add more products...
];

// Load products
function loadProducts() {
    const container = document.getElementById('productsGrid');
    if (!container) return;

    // Apply filters
    let filteredProducts = filterProducts();
    
    // Calculate pagination
    const totalProducts = filteredProducts.length;
    const totalPages = Math.ceil(totalProducts / currentFilters.pageSize);
    const startIndex = (currentFilters.page - 1) * currentFilters.pageSize;
    const endIndex = startIndex + currentFilters.pageSize;
    const paginatedProducts = filteredProducts.slice(startIndex, endIndex);

    // Render products
    if (paginatedProducts.length === 0) {
        container.innerHTML = `
            <div style="grid-column: 1 / -1; text-align: center; padding: 3rem; color: #666;">
                <i class="fas fa-search" style="font-size: 4rem; margin-bottom: 1rem; color: #ddd;"></i>
                <h3>Ürün bulunamadı</h3>
                <p>Arama kriterlerinize uygun ürün bulunamadı. Filtreleri değiştirmeyi deneyin.</p>
                <button class="btn-primary" onclick="clearAllFilters()">
                    Filtreleri Temizle
                </button>
            </div>
        `;
    } else {
        container.innerHTML = paginatedProducts.map(createProductCard).join('');
    }

    // Update UI
    updateResultsCount(totalProducts);
    updatePagination(totalPages);
}

function filterProducts() {
    return sampleProducts.filter(product => {
        // Search term filter
        if (currentFilters.searchTerm) {
            const searchLower = currentFilters.searchTerm.toLowerCase();
            const matchesSearch = product.name.toLowerCase().includes(searchLower) ||
                                product.description.toLowerCase().includes(searchLower) ||
                                product.brand.toLowerCase().includes(searchLower);
            if (!matchesSearch) return false;
        }

        // Category filter
        if (currentFilters.category && product.category !== currentFilters.category) {
            return false;
        }

        // Brand filter
        if (currentFilters.brand && product.brand !== currentFilters.brand) {
            return false;
        }

        // Price range filter
        if (currentFilters.minPrice !== null && product.price < currentFilters.minPrice) {
            return false;
        }
        if (currentFilters.maxPrice !== null && product.price > currentFilters.maxPrice) {
            return false;
        }

        // Stock filter
        if (currentFilters.inStock === 'instock' && product.stock <= 0) {
            return false;
        }
        if (currentFilters.inStock === 'outofstock' && product.stock > 0) {
            return false;
        }

        return true;
    }).sort((a, b) => {
        let aValue, bValue;

        switch (currentFilters.sortField) {
            case 'name':
                aValue = a.name.toLowerCase();
                bValue = b.name.toLowerCase();
                break;
            case 'price':
                aValue = a.price;
                bValue = b.price;
                break;
            case 'brand':
                aValue = a.brand.toLowerCase();
                bValue = b.brand.toLowerCase();
                break;
            case 'category':
                aValue = a.category.toLowerCase();
                bValue = b.category.toLowerCase();
                break;
            default:
                aValue = a.name.toLowerCase();
                bValue = b.name.toLowerCase();
        }

        if (currentFilters.sortDirection === 'desc') {
            return aValue > bValue ? -1 : aValue < bValue ? 1 : 0;
        } else {
            return aValue < bValue ? -1 : aValue > bValue ? 1 : 0;
        }
    });
}

function createProductCard(product) {
    return `
        <div class="product-card">
            <div class="product-image">
                <img src="${product.imageUrl}" alt="${product.name}" loading="lazy">
                ${product.badge ? `<div class="product-badge">${product.badge}</div>` : ''}
                ${product.stock <= 0 ? '<div class="product-badge" style="background: #dc3545;">Stokta Yok</div>' : ''}
            </div>
            <div class="product-info">
                <div class="product-brand">${product.brand.charAt(0).toUpperCase() + product.brand.slice(1)}</div>
                <h3 class="product-title">${product.name}</h3>
                <div class="product-price">
                    <span class="current-price">${formatPrice(product.price)}</span>
                    ${product.originalPrice ? `<span class="original-price">${formatPrice(product.originalPrice)}</span>` : ''}
                </div>
                <div class="product-rating">
                    ${createStarRating(product.rating)}
                    <span class="rating-count">(${product.reviewCount})</span>
                </div>
                <div class="product-actions">
                    <button class="btn-add-cart" 
                            onclick="addToCart(${product.id})" 
                            ${product.stock <= 0 ? 'disabled' : ''}>
                        <i class="fas fa-shopping-cart"></i>
                        ${product.stock <= 0 ? 'Stokta Yok' : 'Sepete Ekle'}
                    </button>
                    <button class="btn-wishlist ${wishlist.includes(product.id) ? 'active' : ''}" 
                            onclick="toggleWishlist(${product.id})">
                        <i class="fa${wishlist.includes(product.id) ? 's' : 'r'} fa-heart"></i>
                    </button>
                </div>
            </div>
        </div>
    `;
}

function createStarRating(rating) {
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 !== 0;
    const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

    let starsHTML = '';
    
    for (let i = 0; i < fullStars; i++) {
        starsHTML += '<i class="fas fa-star"></i>';
    }
    
    if (hasHalfStar) {
        starsHTML += '<i class="fas fa-star-half-alt"></i>';
    }
    
    for (let i = 0; i < emptyStars; i++) {
        starsHTML += '<i class="far fa-star"></i>';
    }

    return `<div class="stars">${starsHTML}</div>`;
}

function formatPrice(price) {
    return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(price);
}

function updateResultsCount(totalCount) {
    const resultsCount = document.getElementById('resultsCount');
    if (resultsCount) {
        const startIndex = (currentFilters.page - 1) * currentFilters.pageSize + 1;
        const endIndex = Math.min(currentFilters.page * currentFilters.pageSize, totalCount);
        
        if (totalCount === 0) {
            resultsCount.textContent = '0 ürün gösteriliyor';
        } else {
            resultsCount.textContent = `${startIndex}-${endIndex} arası, toplam ${totalCount} ürün`;
        }
    }
}

function updatePagination(totalPages) {
    const pagination = document.getElementById('pagination');
    if (!pagination || totalPages <= 1) {
        if (pagination) pagination.innerHTML = '';
        return;
    }

    let paginationHTML = '';

    // Previous button
    paginationHTML += `
        <button ${currentFilters.page === 1 ? 'disabled' : ''} 
                onclick="changePage(${currentFilters.page - 1})">
            <i class="fas fa-chevron-left"></i>
        </button>
    `;

    // Page numbers
    const startPage = Math.max(1, currentFilters.page - 2);
    const endPage = Math.min(totalPages, currentFilters.page + 2);

    for (let i = startPage; i <= endPage; i++) {
        paginationHTML += `
            <button ${i === currentFilters.page ? 'class="active"' : ''} 
                    onclick="changePage(${i})">
                ${i}
            </button>
        `;
    }

    // Next button
    paginationHTML += `
        <button ${currentFilters.page === totalPages ? 'disabled' : ''} 
                onclick="changePage(${currentFilters.page + 1})">
            <i class="fas fa-chevron-right"></i>
        </button>
    `;

    pagination.innerHTML = paginationHTML;
}

function changePage(page) {
    currentFilters.page = page;
    loadProducts();
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function toggleFilterPanel() {
    const panel = document.getElementById('filterPanel');
    if (panel) {
        panel.classList.toggle('collapsed');
    }
}

function applyFilters() {
    // Collect filter values
    currentFilters.searchTerm = document.getElementById('searchTerm')?.value.trim() || '';
    currentFilters.category = document.getElementById('categoryFilter')?.value || '';
    currentFilters.brand = document.getElementById('brandFilter')?.value || '';
    currentFilters.minPrice = document.getElementById('minPrice')?.value ? parseInt(document.getElementById('minPrice').value) : null;
    currentFilters.maxPrice = document.getElementById('maxPrice')?.value ? parseInt(document.getElementById('maxPrice').value) : null;
    currentFilters.inStock = document.getElementById('stockFilter')?.value || '';
    currentFilters.sortField = document.getElementById('sortField')?.value || 'name';
    currentFilters.sortDirection = document.getElementById('sortDirection')?.value || 'asc';
    currentFilters.page = 1; // Reset to first page

    loadProducts();
    showAlert('Filtreler uygulandı', 'success');
}

function clearAllFilters() {
    // Reset all filter inputs
    const inputs = ['searchTerm', 'categoryFilter', 'brandFilter', 'minPrice', 'maxPrice', 'stockFilter'];
    inputs.forEach(id => {
        const element = document.getElementById(id);
        if (element) element.value = '';
    });
    
    const sortField = document.getElementById('sortField');
    const sortDirection = document.getElementById('sortDirection');
    if (sortField) sortField.value = 'name';
    if (sortDirection) sortDirection.value = 'asc';

    // Reset filter object
    currentFilters = {
        searchTerm: '',
        category: '',
        brand: '',
        minPrice: null,
        maxPrice: null,
        inStock: '',
        sortField: 'name',
        sortDirection: 'asc',
        page: 1,
        pageSize: 6
    };

    loadProducts();
    showAlert('Tüm filtreler temizlendi', 'info');
}

function toggleView(view) {
    const productsGrid = document.getElementById('productsGrid');
    const viewButtons = document.querySelectorAll('.view-btn');
    
    viewButtons.forEach(btn => {
        btn.classList.toggle('active', btn.dataset.view === view);
    });

    if (productsGrid) {
        productsGrid.classList.toggle('list-view', view === 'list');
    }
}

function toggleWishlist(productId) {
    if (!currentUser) {
        showAlert('Favorilere eklemek için giriş yapmalısınız.', 'warning');
        showModal('loginModal');
        return;
    }

    const isInWishlist = wishlist.includes(productId);
    const product = sampleProducts.find(p => p.id === productId);

    if (isInWishlist) {
        wishlist = wishlist.filter(id => id !== productId);
        showAlert('Favorilerden kaldırıldı.', 'info');
    } else {
        wishlist.push(productId);
        showAlert(`${product.name} favorilere eklendi!`, 'success');
    }

    localStorage.setItem('wishlist', JSON.stringify(wishlist));
    loadProducts(); // Refresh to update wishlist buttons
}

// Initialize products page
document.addEventListener('DOMContentLoaded', function() {
    // Check for URL parameters
    const urlParams = new URLSearchParams(window.location.search);
    const category = urlParams.get('category');
    const search = urlParams.get('search');

    // Initialize filters
    currentFilters = {
        category: '',
        searchTerm: '',
        brand: '',
        minPrice: '',
        maxPrice: ''
    };

    if (category) {
        currentFilters.category = decodeURIComponent(category);
        const categoryFilter = document.getElementById('categoryFilter');
        if (categoryFilter) {
            categoryFilter.value = currentFilters.category;
            // Trigger filter change event
            const event = new Event('change');
            categoryFilter.dispatchEvent(event);
        }
    }

    if (search) {
        currentFilters.searchTerm = decodeURIComponent(search);
        const searchTerm = document.getElementById('searchTerm');
        if (searchTerm) {
            searchTerm.value = currentFilters.searchTerm;
            // Trigger search input event
            const event = new Event('input');
            searchTerm.dispatchEvent(event);
        }
    }

    // Load products with initial filters
    loadProducts();
}); 