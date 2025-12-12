import { Component, inject, input, OnInit, output } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, JsonPipe],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register implements OnInit {
  
  private accountService = inject(AccountService)
  // membersFromHome = input.required<User[]>(); // this is the other way of getting the data from the parent.
  cancelRegister = output<boolean>();
  protected creds = {} as RegisterCreds
  protected registerForm: FormGroup =  new FormGroup({});

  ngOnInit(): void {
    this.initializeRegisterForm();
  }

  initializeRegisterForm(){
    this.registerForm = new FormGroup({
      email: new FormControl(),
      displayName: new FormControl(),
      password: new FormControl(),
      confirmPassword: new FormControl()
    })
  }
  
  register(){

    console.log(this.registerForm.value);
    // this.accountService.register(this.creds).subscribe({
    //   next: response => {
    //     console.log(response);
    //     this.cancel();
    //   },
    //   error: error => console.log(error)
    // })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }
}
