import { UrlConstants } from './../../../shared/constants/url.constants';
import { TokenStorageService } from './../../../shared/services/token-storage.service';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AdminApiAuthApiClient, AuthenticatedResult, LoginRequest } from 'src/app/api/admin-api.service.generated';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  constructor(private fb: FormBuilder, private authApiClient: AdminApiAuthApiClient, private alertService: AlertService, private router: Router, private tokenService: TokenStorageService) {
    this.loginForm = this.fb.group({
      userName: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
   }

   login(){
    var request: LoginRequest = new LoginRequest({
      userName: this.loginForm.controls['userName'].value,
      password: this.loginForm.controls['password'].value
    });

    this.authApiClient.login(request).subscribe({
      next: (res: AuthenticatedResult) => {
        this.tokenService.saveToken(res.token);
        this.tokenService.saveRefreshToken(res.refreshToken);
        this.tokenService.saveUser(res);
        this.router.navigate([UrlConstants.HOME]);
      },
      error: (err: any)=> {
        console.log(err);
        this.alertService.showError('Đăng nhập không thành công');
      }
    })
   }

}
