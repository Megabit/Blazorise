import "./vendors/jsencrypt.min.js?v=1.1.2.0";

export function verify(publicKey, content, signature) {
    try {
        const jsEncrypt = new JSEncrypt();
        jsEncrypt.setPublicKey(publicKey);
        return jsEncrypt.verify(content, signature, CryptoJS.SHA512);
    } catch (error) {
        console.error(error);
    }

    return false;
}