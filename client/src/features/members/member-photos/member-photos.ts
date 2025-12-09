import { Component, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { Photo } from '../../../types/member';
import { AsyncPipe } from '@angular/common';
import { ImageUpload } from '../../../shared/image-upload/image-upload';

@Component({
  selector: 'app-member-photos',
  imports: [AsyncPipe, ImageUpload],
  templateUrl: './member-photos.html',
  styleUrl: './member-photos.css'
})
export class MemberPhotos implements OnInit {
  protected memberService = inject(MemberService);
  private route = inject(ActivatedRoute);

  // protected photos$?: Observable<Photo[]>;
  protected photos = signal<Photo[]>([])
  protected loading = signal(false);

  constructor() {
    // const memberId = this.route.parent?.snapshot.paramMap.get('id');
    // if(memberId){
    //   // this.photos$ = this.memberService.getMemberPhotos(memberId);
    //   this.memberService.getMemberPhotos(memberId).subscribe({
    //     next: photos => this.photos$?.set(photos)
    //   })
    // }
  }
  ngOnInit(): void {
    const memberId = this.route.parent?.snapshot.paramMap.get('id');
    if (memberId) {
      this.memberService.getMemberPhotos(memberId).subscribe({
        next: photos => this.photos?.set(photos)
      })
    }
  }

  // get photoMocks() {
  //   return Array.from({ length: 20 }, (_, i) => ({
  //     url: '/user.png'
  //   }))
  // }

  onUploadImage(file: File) {
    this.loading.set(true);
    this.memberService.uploadPhoto(file).subscribe({
      next: photo => {
        this.memberService.editMode.set(false);
        this.loading.set(false);
        this.photos?.update(photos => [...photos, photo])
      },
      error: error => {
        console.log('Error uploading image', error);
        this.loading.set(false);
      }
    })
  }
}
