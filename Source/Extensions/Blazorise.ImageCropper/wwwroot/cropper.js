import "./vendors/cropper.min.js?v=1.5.11";

class ImageCropper {
    constructor(image) {
        this.image = image;
    }

    update( options ) {
        if (this.disposed)
            return;
        if (this.cropper) {
            this.cropper.destroy();
        }
        this.cropper = new Cropper( this.image, options );
    }

    crop( width, height ) {
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