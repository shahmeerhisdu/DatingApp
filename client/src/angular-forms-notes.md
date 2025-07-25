## Angular Template Driven Forms

<form class="flex items-center gap-3"> to turn this to angular template driven form we need to add the template reference variable to our form

<input type="text" class="input" placeholder="Email"> to make it two way binding we need to add the [(ngModel)], like this <input [(ngModel)]="creds.email" name="email" type="text" class="input" placeholder="Email"> and we added the name in angular for the tracking purpose.

## Reactive Forms or Component Driven Forms