// Home page specific functionality

// Initialize home page
document.addEventListener('DOMContentLoaded', function() {
    console.log('Ana sayfa yüklendi');
    
    // Smooth scrolling for anchor links
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    anchorLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Initialize featured products slider
    initFeaturedProductsSlider();

    // Initialize category cards
    initCategoryCards();

    // Initialize newsletter form
    initNewsletterForm();
});

// Featured Products Slider
function initFeaturedProductsSlider() {
    const slider = document.querySelector('.featured-products-slider');
    if (!slider) return;

    let currentSlide = 0;
    const slides = slider.querySelectorAll('.product-card');
    const totalSlides = slides.length;
    const slidesPerView = getSlidesPerView();

    // Create navigation buttons
    const prevBtn = document.createElement('button');
    prevBtn.className = 'slider-nav prev';
    prevBtn.innerHTML = '<i class="fas fa-chevron-left"></i>';
    
    const nextBtn = document.createElement('button');
    nextBtn.className = 'slider-nav next';
    nextBtn.innerHTML = '<i class="fas fa-chevron-right"></i>';

    slider.appendChild(prevBtn);
    slider.appendChild(nextBtn);

    // Update slider position
    function updateSlider() {
        const offset = currentSlide * -100;
        slider.querySelector('.slider-track').style.transform = `translateX(${offset}%)`;
        
        // Update button states
        prevBtn.disabled = currentSlide === 0;
        nextBtn.disabled = currentSlide >= totalSlides - slidesPerView;
    }

    // Navigation button events
    prevBtn.addEventListener('click', () => {
        if (currentSlide > 0) {
            currentSlide--;
            updateSlider();
        }
    });

    nextBtn.addEventListener('click', () => {
        if (currentSlide < totalSlides - slidesPerView) {
            currentSlide++;
            updateSlider();
        }
    });

    // Responsive slides per view
    function getSlidesPerView() {
        if (window.innerWidth < 576) return 1;
        if (window.innerWidth < 992) return 2;
        return 3;
    }

    // Handle window resize
    window.addEventListener('resize', () => {
        const newSlidesPerView = getSlidesPerView();
        if (currentSlide > totalSlides - newSlidesPerView) {
            currentSlide = Math.max(0, totalSlides - newSlidesPerView);
        }
        updateSlider();
    });

    // Initialize slider
    updateSlider();
}

// Category Cards
function initCategoryCards() {
    const categoryCards = document.querySelectorAll('.category-card');
    
    categoryCards.forEach(card => {
        // Hover effect
        card.addEventListener('mouseenter', function() {
            this.classList.add('hover');
        });

        card.addEventListener('mouseleave', function() {
            this.classList.remove('hover');
        });

        // Click event
        card.addEventListener('click', function() {
            const categoryId = this.dataset.categoryId;
            if (categoryId) {
                window.location.href = `/Home/Products?category=${categoryId}`;
            }
        });
    });
}

// Newsletter Form
function initNewsletterForm() {
    const form = document.getElementById('newsletterForm');
    if (!form) return;

    form.addEventListener('submit', function(e) {
        e.preventDefault();
        const email = this.querySelector('input[type="email"]').value;
        
        if (email) {
            // API call would go here
            subscribeNewsletter(e);
        }
    });
}

// Hero Section Parallax
function initHeroParallax() {
    const hero = document.querySelector('.hero-section');
    if (!hero) return;

    window.addEventListener('scroll', function() {
        const scrolled = window.pageYOffset;
        const parallaxElements = hero.querySelectorAll('.parallax');
        
        parallaxElements.forEach(element => {
            const speed = element.dataset.speed || 0.5;
            element.style.transform = `translateY(${scrolled * speed}px)`;
        });
    });
}

// Initialize hero parallax if hero section exists
if (document.querySelector('.hero-section')) {
    initHeroParallax();
} 