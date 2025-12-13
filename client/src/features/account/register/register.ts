import { Component, inject, input, OnInit, output, signal, Signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { RegisterCreds, User } from '../../../types/user';
import { AccountService } from '../../../core/services/account-service';
import { JsonPipe } from '@angular/common';
import { TextInput } from "../../../shared/text-input/text-input";

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, JsonPipe, TextInput],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  private accountService = inject(AccountService)
  private fb = inject(FormBuilder);
  // membersFromHome = input.required<User[]>(); // this is the other way of getting the data from the parent.
  cancelRegister = output<boolean>();
  protected creds = {} as RegisterCreds
  protected credentialsForm: FormGroup;
  protected profileForm: FormGroup;
  protected currentStep =  signal(1);

  constructor() {
    this.credentialsForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      displayName: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });

    this.profileForm = this.fb.group({
      gender: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
    })

    //custom validation only occurs on the confirm password if we correct the password after confirm password we will get invalid because the confirm password does not know that the password field has been changed so we need some mechanism for that as well that is done in here.

    this.credentialsForm.controls['password'].valueChanges.subscribe(() => {
      this.credentialsForm.controls['confirmPassword'].updateValueAndValidity();
    })

  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const parent = control.parent;

      if (!parent) return null;

      const matchValue = parent.get(matchTo)?.value;

      return control.value === matchValue ? null : { passwordMismatch: true }
    }
  }

  nextStep(){
    if(this.credentialsForm.valid){
      this.currentStep.update(prevStep => prevStep + 1);
    }
  }

  prevStep(){
    this.currentStep.update(prevStep => prevStep - 1);
  }

  register() {

    if(this.profileForm.valid && this.credentialsForm.valid){
      const formData = {... this.credentialsForm.value, ...this.profileForm.value};
      console.log("Form Data: ", formData);
    }


    // this.accountService.register(this.creds).subscribe({
    //   next: response => {
    //     console.log(response);
    //     this.cancel();
    //   },
    //   error: error => console.log(error)
    // })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
