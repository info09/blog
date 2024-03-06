import { UrlConstants } from './../../../shared/constants/url.constants';
import { TokenStorageService } from './../../../shared/services/token-storage.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiAuthApiClient, AuthenticatedResult, LoginRequest } from 'src/app/api/admin-api.service.generated';
import { AlertService } from 'src/app/shared/services/alert.service';
import { BroadcastService } from 'src/app/shared/services/broadcast.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnDestroy, OnInit {
  loginForm: FormGroup;
  private ngUnsubscribe = new Subject<void>();
  loading = false;
  constructor(private fb: FormBuilder, private authApiClient: AdminApiAuthApiClient, private alertService: AlertService, private router: Router, private tokenService: TokenStorageService, private broadcastService: BroadcastService) {
    this.loginForm = this.fb.group({
      userName: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
  }
  ngOnInit(): void {
    this.broadcastService.httpError.asObservable().subscribe(value => {
      this.loading = false
    })
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  login() {
    this.loading = true;
    var request: LoginRequest = new LoginRequest({
      userName: this.loginForm.controls['userName'].value,
      password: this.loginForm.controls['password'].value
    });

    this.authApiClient.login(request).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
      next: (res: AuthenticatedResult) => {
        this.tokenService.saveToken(res.token);
        this.tokenService.saveRefreshToken(res.refreshToken);
        this.tokenService.saveUser(res);
        this.router.navigate([UrlConstants.HOME]);
      },
      error: (err: any) => {
        console.log(err);
        this.alertService.showError('Đăng nhập không thành công');
        this.loading = false;
      }
    })
  }

}
