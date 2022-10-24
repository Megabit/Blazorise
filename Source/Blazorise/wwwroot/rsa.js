import "./vendors/jsencrypt.js?v=1.1.2.0";
import "./vendors/sha512.js?v=1.1.2.0";

export function verify(publicKey, content, signature) {
    try {
        const jsEncrypt = new JSEncrypt();
        jsEncrypt.setPublicKey(publicKey);

        const verified = jsEncrypt.verify(content, signature, sha512);

        if (verified) {
            return true;
        }
    } catch (error) {
        console.error(error);
    }

    return false;
}