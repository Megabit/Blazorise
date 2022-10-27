import "./vendors/cropper.min.js?v=1.5.11";

class ImageCropper {
    constructor(image) {
        this.image = image;
    }
    update(ratio) {
        if (this.disposed)
            return;
        if (this.cropper) {
            this.cropper.destroy();
        }
        this.cropper = new Cropper(this.image, {
            aspectRatio: ratio,
            viewMode: 1
        });
    }
    crop(width, height) {
        if (this.disposed)
            return "";
        const canvas = this.cropper.getCroppedCanvas({
            width: width,
            height: height,
        });
        if (!canvas) {
            return "";
        }
        return canvas.toDataURL();
    }
    destroy() {
        if (this.disposed)
            return;
        this.disposed = true;
        this.cropper.destroy();
        this.cropper = undefined;
    }
}

export function createCropper(image) {
    return new ImageCropper(image);
}

export function getImageUrl(inputId) {
    const input = document.getElementById(inputId);
    if (!input || input.files.length === 0) {
        return undefined;
    }
    var file = input.files[0];
    return URL.createObjectURL(file);
}
