// API Base URL
const API_BASE_URL = window.location.origin + '/api';

// Global variables
let cart = JSON.parse(localStorage.getItem('cart') || '[]');
let wishlist = JSON.parse(localStorage.getItem('wishlist') || '[]');
let currentUser = JSON.parse(localStorage.getItem('user') || 'null');

// API Service Class
class ApiService {
    static async get(endpoint) {
        try {
            const response = await fetch(`${API_BASE_URL}${endpoint}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include'
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error('API GET Error:', error);
            throw error;
        }
    }

    static async post(endpoint, data) {
        try {
            const response = await fetch(`${API_BASE_URL}${endpoint}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include',
                body: JSON.stringify(data)
            });
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            return await response.json();
        } catch (error) {
            console.error('API POST Error:', error);
            throw error;
        }
    }
}

// Cart functions
function addToCart(productId) {
    // Implement cart functionality
    showAlert('Ürün sepete eklendi!', 'success');
    updateCartDisplay();
}

function updateCartDisplay() {
    const cartCount = document.getElementById('cartCount');
    const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
    
    if (cartCount) {
        cartCount.textContent = totalItems;
        cartCount.style.display = totalItems > 0 ? 'flex' : 'none';
    }
}

// Modal functions
function showModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('show');
        modal.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    }
}

function hideModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('show');
        modal.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
}

// Alert function
function showAlert(message, type = 'info') {
    const alert = document.createElement('div');
    alert.className = `alert alert-${type}`;
    alert.innerHTML = `
        <i class="fas fa-${getAlertIcon(type)}"></i>
        <span>${message}</span>
    `;

    document.body.appendChild(alert);

    // Auto remove after 5 seconds
    setTimeout(() => {
        if (alert.parentNode) {
            alert.parentNode.removeChild(alert);
        }
    }, 5000);

    // Click to close
    alert.addEventListener('click', () => {
        if (alert.parentNode) {
            alert.parentNode.removeChild(alert);
        }
    });
}

function getAlertIcon(type) {
    const icons = {
        success: 'check-circle',
        error: 'exclamation-circle',
        warning: 'exclamation-triangle',
        info: 'info-circle'
    };
    return icons[type] || 'info-circle';
}

// Newsletter function
function subscribeNewsletter(event) {
    event.preventDefault();
    const email = event.target.querySelector('input[type="email"]').value;
    
    if (email) {
        showAlert('Bülten aboneliğiniz başarıyla oluşturuldu!', 'success');
        event.target.reset();
    }
}

// Authentication functions
function handleLogin(event) {
    event.preventDefault();
    
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    // Simulate login
    if (email && password) {
        currentUser = {
            id: 1,
            firstName: 'Ahmet',
            lastName: 'Yılmaz',
            email: email,
            role: email.includes('admin') ? 'admin' : 'user'
        };
        
        localStorage.setItem('user', JSON.stringify(currentUser));
        hideModal('loginModal');
        updateAuthDisplay();
        showAlert('Giriş başarılı!', 'success');
    }
}

function updateAuthDisplay() {
    const loginBtn = document.getElementById('loginBtn');
    
    if (loginBtn) {
        if (currentUser) {
            loginBtn.innerHTML = `
                <i class="fas fa-user"></i>
                <span>${currentUser.firstName}</span>
            `;
        } else {
            loginBtn.innerHTML = `
                <i class="fas fa-user"></i>
                <span>Giriş</span>
            `;
        }
    }
}

// Initialize common functionality
document.addEventListener('DOMContentLoaded', function() {
    updateCartDisplay();
    updateAuthDisplay();
    
    // Login button event
    const loginBtn = document.getElementById('loginBtn');
    if (loginBtn) {
        loginBtn.addEventListener('click', function() {
            if (currentUser) {
                // Show user menu
                showAlert('Kullanıcı menüsü yakında eklenecek', 'info');
            } else {
                showModal('loginModal');
            }
        });
    }

    // Cart button event
    const cartBtn = document.getElementById('cartBtn');
    if (cartBtn) {
        cartBtn.addEventListener('click', function() {
            window.location.href = '/cart';
        });
    }

    // Search functionality
    const searchBtn = document.getElementById('searchBtn');
    const searchInput = document.getElementById('searchInput');
    
    if (searchBtn) {
        searchBtn.addEventListener('click', performSearch);
    }
    
    if (searchInput) {
        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                performSearch();
            }
        });
    }

    // Close modals on outside click
    window.addEventListener('click', function(e) {
        if (e.target.classList.contains('modal')) {
            hideModal(e.target.id);
        }
    });
});

function performSearch() {
    const query = document.getElementById('searchInput').value.trim();
    if (query) {
        window.location.href = `/Home/Products?search=${encodeURIComponent(query)}`;
    }
} 