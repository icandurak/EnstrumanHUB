// Firebase yapılandırması
const firebaseConfig = {
    apiKey: "AIzaSyAS0-UuWWFNMSvnNGEzY0pdBGxhZLEqjnA",
    authDomain: "enstrumand2.firebaseapp.com",
    projectId: "enstrumand2",
    storageBucket: "enstrumand2.firebasestorage.app",
    messagingSenderId: "983179534658",
    appId: "1:983179534658:web:bdccea5d518c26d8dbe8db",
    measurementId: "G-7VXJLWTP28"
};

// Firebase'i başlat
firebase.initializeApp(firebaseConfig);

// Auth işlemleri için yardımcı fonksiyonlar
const auth = {
    // Kullanıcı girişi
    async login(email, password) {
        try {
            const userCredential = await firebase.auth().signInWithEmailAndPassword(email, password);
            const idToken = await userCredential.user.getIdToken();
            localStorage.setItem('idToken', idToken);
            return userCredential.user;
        } catch (error) {
            console.error('Giriş hatası:', error);
            throw error;
        }
    },

    // Kullanıcı kaydı
    async register(email, password) {
        try {
            const userCredential = await firebase.auth().createUserWithEmailAndPassword(email, password);
            const idToken = await userCredential.user.getIdToken();
            localStorage.setItem('idToken', idToken);
            return userCredential.user;
        } catch (error) {
            console.error('Kayıt hatası:', error);
            throw error;
        }
    },

    // Çıkış yap
    async logout() {
        try {
            await firebase.auth().signOut();
            localStorage.removeItem('idToken');
        } catch (error) {
            console.error('Çıkış hatası:', error);
            throw error;
        }
    },

    // Mevcut kullanıcıyı kontrol et
    getCurrentUser() {
        return new Promise((resolve, reject) => {
            const unsubscribe = firebase.auth().onAuthStateChanged(user => {
                unsubscribe();
                resolve(user);
            }, reject);
        });
    },

    // API istekleri için token al
    async getIdToken() {
        const user = firebase.auth().currentUser;
        if (user) {
            return await user.getIdToken(true);
        }
        return null;
    }
};

// API istekleri için axios interceptor
axios.interceptors.request.use(async config => {
    const token = await auth.getIdToken();
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Auth state değişikliklerini dinle
firebase.auth().onAuthStateChanged(user => {
    if (user) {
        console.log('Kullanıcı giriş yaptı:', user.email);
        // Giriş yapıldığında yapılacak işlemler
    } else {
        console.log('Kullanıcı çıkış yaptı');
        // Çıkış yapıldığında yapılacak işlemler
    }
}); 