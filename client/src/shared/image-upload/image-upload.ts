import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css'
})
export class ImageUpload {

  protected imageSrc = signal< string | ArrayBuffer | null | undefined>(null);
  protected isDragging = false;
  protected fileToUpload: File | null = null;

  //notify the member photos about the uploaded image so we need to use the output property
  uplaodFile = output<File>();
  loading = input<boolean>(false);

  //We need some methods to handle the drag events.

  onDragOver(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }
  
  onDrop(event: DragEvent){
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    if(event.dataTransfer?.files.length){
      const file = event.dataTransfer.files[0]; //because we are only allowing to upload only one image at a time.
      this.previewImage(file);
      this.fileToUpload = file;
    }
  }

  onCancel(){
    this.fileToUpload = null;
    this.imageSrc.set(null);
  }

  onUploadFile(){
    if(this.fileToUpload){
      this.uplaodFile.emit(this.fileToUpload);
    }
  }

  private previewImage(file: File){
    const reader = new FileReader();
    reader.onload = (e) => this.imageSrc.set(e.target?.result);
    reader.readAsDataURL(file);
  }
}
