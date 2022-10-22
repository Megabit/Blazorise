import "./vendors/jsencrypt.min.js";

export function verify(publicKeyBase64, bytesContent, bytesSignature) {
    try {
        console.log(bytesContent);
        console.log(bytesSignature);
        // Verify with the public key...
        var jsEncrypt = new JSEncrypt();
        jsEncrypt.setPublicKey(`MIIBCgKCAQEAuWaYibdLKZjYHDBS6K2EWBV9TSWhMiJU/67jN1keOphiINQVzk6RYCuazPUyFrZwx6iCwlLMBMxRB7wEiRITIhEOULlRDK2o2AwFTCG7px3SCVNDoMi0C6zrj090iBhbGDUZpX9TA06XWEq+LUzIQncNa4OPtkqIWxAGVAKxQr9CbAYIrOEPA3cANQQUUIjCn2HjhojTzWzHhFEB245epO7TWiuo8KQGxVUQXiWHkJuX7nLsgkd3CeBIgqwh+trm/JRxCiY7TkghXPY+N+TIOQPBrTO3cHUnuyGEPloU0J7B5RToqwHzwdjaz2HKA5cQAw1xnHmiYU1ixxrWDphTKQIDAQAB`);
        var verified = jsEncrypt.verify(bytesContent, bytesSignature, CryptoJS.SHA256);


        // Now a simple check to see if the round-trip worked.
        if (verified) {
            console.log('It works!!!');
            return true;
        }
        else {
            console.log('Something went wrong....');
        }
    } catch (error) {
        console.error(error);
    }


    return false;
}

//import "./vendors/forge.js";

//export function verify(publicKeyBase64, bytesContent, bytesSignature) {
//    try {


//        //        var publicKey = forge.pki.publicKeyFromPem(`-----BEGIN PUBLIC KEY-----
//        //MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDGsBK/1ZYwqcZ3fFBaZ4EHZAE9
//        //wjQeauhLFJBRvDhLltVpCEZXpWn56ERpC0o2qlmlSGIzWRbCWQEegn2qCOPdKO6w
//        //hWqNScO5YrN1/Mx7GCBV5VcR4h/NoV+QU2c4YCJxDMu0kdEiOHLOFyTBRYk3hM/M
//        //tGpLhCz9OwgI5b6spQIDAQAB
//        //-----END PUBLIC KEY-----`);

//        //        var rsa = forge.pki.rsa;
//        //        var md = forge.md.sha512.create();
//        //        md.update(bytesContent, 'utf8');
//        //        var verified = publicKey.verify(md.digest().bytes(), base64ArrayBuffer(bytesSignature));




//        //var rsa = forge.pki.rsa;
//        //var publicKey = rsa.setPublicKey(mod, exponent);

//        //var md = forge.md.sha512.create();
//        //md.update(content, 'utf8');
//        //var verified = publicKey.verify(md.digest().bytes(), signature);





//        //var pkstr = '-----BEGIN PUBLIC KEY-----' + publicKeyBase64 + '-----END PUBLIC KEY-----'; // not friendly
//        //var pk = forge.pki.publicKeyFromPem(publicKeyBase64);
//        //var bytes = pk.encrypt("text to protect"); // `bytes` cost me a night
//        //var encrypted = forge.util.encode64(bytes);

//        //var pkstr = '-----BEGIN PUBLIC KEY-----' + publicKeyBase64 + '-----END PUBLIC KEY-----'; // not friendly
//        //var publicKey = forge.pki.publicKeyFromPem(pkstr);



//        //var rsa = forge.pki.rsa;

//        //const publicKey = rsa.setPublicKey(publicKeyBase64);

//        //var md = forge.md.sha512.create();
//        //md.update(content, 'utf8');
//        //var verified = publicKey.verify(md.digest().bytes(), signature);
//    } catch (error) {
//        console.error(error);
//    }
//    return false;
//}

function base64ArrayBuffer(arrayBuffer) {
    var base64 = ''
    var encodings = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'

    var bytes = new Uint8Array(arrayBuffer)
    var byteLength = bytes.byteLength
    var byteRemainder = byteLength % 3
    var mainLength = byteLength - byteRemainder

    var a, b, c, d
    var chunk

    // Main loop deals with bytes in chunks of 3
    for (var i = 0; i < mainLength; i = i + 3) {
        // Combine the three bytes into a single integer
        chunk = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2]

        // Use bitmasks to extract 6-bit segments from the triplet
        a = (chunk & 16515072) >> 18 // 16515072 = (2^6 - 1) << 18
        b = (chunk & 258048) >> 12 // 258048   = (2^6 - 1) << 12
        c = (chunk & 4032) >> 6 // 4032     = (2^6 - 1) << 6
        d = chunk & 63               // 63       = 2^6 - 1

        // Convert the raw binary segments to the appropriate ASCII encoding
        base64 += encodings[a] + encodings[b] + encodings[c] + encodings[d]
    }

    // Deal with the remaining bytes and padding
    if (byteRemainder == 1) {
        chunk = bytes[mainLength]

        a = (chunk & 252) >> 2 // 252 = (2^6 - 1) << 2

        // Set the 4 least significant bits to zero
        b = (chunk & 3) << 4 // 3   = 2^2 - 1

        base64 += encodings[a] + encodings[b] + '=='
    } else if (byteRemainder == 2) {
        chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1]

        a = (chunk & 64512) >> 10 // 64512 = (2^6 - 1) << 10
        b = (chunk & 1008) >> 4 // 1008  = (2^6 - 1) << 4

        // Set the 2 least significant bits to zero
        c = (chunk & 15) << 2 // 15    = 2^4 - 1

        base64 += encodings[a] + encodings[b] + encodings[c] + '='
    }

    return base64
}