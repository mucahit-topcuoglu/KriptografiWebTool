const API_BASE_URL = 'http://localhost:3001/api/crypto';

function toggleTheme() {
    const html = document.documentElement;
    const currentTheme = html.getAttribute('data-theme');
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';
    html.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);
}

document.addEventListener('DOMContentLoaded', () => {
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);
});

async function copyResult(elementId) {
    try {
        const element = document.getElementById(elementId);
        await navigator.clipboard.writeText(element.value);
        showToast('Kopyalandı!');
    } catch (err) {
        showToast('Kopyalama başarısız!');
    }
}

function downloadResult(elementId) {
    try {
        const element = document.getElementById(elementId);
        const text = element.value;
        if (!text) {
            showToast('İndirilecek içerik yok!');
            return;
        }
        const blob = new Blob([text], { type: 'text/plain' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'result.txt';
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        showToast('İndirildi!');
    } catch (err) {
        showToast('İndirme başarısız!');
    }
}

function showToast(message) {
    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => {
        toast.remove();
    }, 2000);
}

async function encryptAES() {
    const text = document.getElementById('aesText').value;
    const key = document.getElementById('aesKey').value;

    if (!text || !key) {
        showToast('Lütfen metin ve anahtar girin!');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/aes/encrypt`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ text, key })
        });

        if (!response.ok) {
            throw new Error('Şifreleme başarısız!');
        }

        const data = await response.json();
        document.getElementById('aesResult').value = data.encryptedText;
    } catch (error) {
        showToast(error.message);
    }
}

async function decryptAES() {
    const text = document.getElementById('aesText').value;
    const key = document.getElementById('aesKey').value;

    if (!text || !key) {
        showToast('Lütfen metin ve anahtar girin!');
        return;
    }

    try {
        const response = await fetch(`${API_BASE_URL}/aes/decrypt`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ text, key })
        });

        if (!response.ok) {
            throw new Error('Çözme başarısız!');
        }

        const data = await response.json();
        document.getElementById('aesResult').value = data.decryptedText;
    } catch (error) {
        showToast(error.message);
    }
}

async function generateRSAKeys() {
    try {
        console.log('RSA Anahtar Oluşturma İsteği');
        
        const response = await fetch(`${API_BASE_URL}/rsa/generate-keys`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const responseData = await response.json();
        console.log('RSA Anahtar Oluşturma Yanıtı:', {
            publicKey: responseData.publicKey ? responseData.publicKey.substring(0, 50) + '...' : null,
            privateKey: responseData.privateKey ? responseData.privateKey.substring(0, 50) + '...' : null
        });

        if (!response.ok) {
            throw new Error(responseData.message || 'Anahtar oluşturma başarısız!');
        }

        if (responseData.publicKey && responseData.privateKey) {
            document.getElementById('rsaPublicKey').value = responseData.publicKey;
            document.getElementById('rsaPrivateKey').value = responseData.privateKey;
            showToast('Anahtar çifti oluşturuldu!');
        } else {
            throw new Error('Geçersiz anahtar yanıtı!');
        }
    } catch (error) {
        console.error('RSA Anahtar Oluşturma Hatası:', error);
        showToast(error.message);
    }
}

async function encryptRSA() {
    const text = document.getElementById('rsaText').value;
    const publicKey = document.getElementById('rsaPublicKey').value;

    if (!text || !publicKey) {
        showToast('Lütfen metin ve public key girin!');
        return;
    }

    try {
        console.log('RSA Şifreleme İsteği:', { text, publicKey: publicKey.substring(0, 50) + '...' });
        
        const response = await fetch(`${API_BASE_URL}/rsa/encrypt-text`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ text, publicKey })
        });

        const responseData = await response.json();
        console.log('RSA Şifreleme Yanıtı:', responseData);

        if (!response.ok) {
            throw new Error(responseData.message || 'Şifreleme başarısız!');
        }

        if (responseData.encryptedText) {
            document.getElementById('rsaResult').value = responseData.encryptedText;
            showToast('Metin şifrelendi!');
        } else {
            throw new Error('Geçersiz şifreleme yanıtı!');
        }
    } catch (error) {
        console.error('RSA Şifreleme Hatası:', error);
        showToast(error.message);
    }
}


async function decryptRSA() {
    const text = document.getElementById('rsaText').value;
    const privateKey = document.getElementById('rsaPrivateKey').value;

    if (!text || !privateKey) {
        showToast('Lütfen metin ve private key girin!');
        return;
    }

    try {
        console.log('RSA Çözme İsteği:', { text, privateKey: privateKey.substring(0, 50) + '...' });
        
        const response = await fetch(`${API_BASE_URL}/rsa/decrypt-text`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ text, privateKey })
        });

        const responseData = await response.json();
        console.log('RSA Çözme Yanıtı:', responseData);

        if (!response.ok) {
            throw new Error(responseData.message || 'Çözme başarısız!');
        }

        if (responseData.decryptedText) {
            document.getElementById('rsaResult').value = responseData.decryptedText;
            showToast('Metin çözüldü!');
        } else {
            throw new Error('Geçersiz çözme yanıtı!');
        }
    } catch (error) {
        console.error('RSA Çözme Hatası:', error);
        showToast(error.message);
    }
}

async function computeHash() {
    const text = document.getElementById('hashText').value;
    const file = document.getElementById('hashFile').files[0];

    if (!text && !file) {
        showToast('Lütfen metin girin veya dosya seçin!');
        return;
    }

    try {
        let response;
        if (file) {
            const formData = new FormData();
            formData.append('file', file);
            response = await fetch(`${API_BASE_URL}/sha256/file`, {
                method: 'POST',
                body: formData
            });
        } else {
            response = await fetch(`${API_BASE_URL}/sha256/text`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ text })
            });
        }

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Hash hesaplama başarısız!');
        }

        const data = await response.json();
        if (data.hash) {
            document.getElementById('hashResult').value = data.hash;
            showToast('Hash hesaplandı!');
        } else {
            throw new Error('Geçersiz hash yanıtı!');
        }
    } catch (error) {
        showToast(error.message);
        console.error('Hash Computation Error:', error);
    }
} 