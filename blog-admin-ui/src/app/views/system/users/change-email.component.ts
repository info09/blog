import { AdminApiUserApiClient, UserDto } from './../../../api/admin-api.service.generated';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Component, EventEmitter, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { Subject, takeUntil } from "rxjs";

@Component({
    templateUrl: 'change-email.component.html'
})
export class ChangeEmailComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();
    // Default
    public blockedPanelDetail: boolean = false;
    public form: FormGroup;
    public title: string;
    public btnDisabled = false;
    public saveBtnName: string;
    public closeBtnName: string;
    public email: string | undefined;
    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig, private userService: AdminApiUserApiClient, private fb: FormBuilder) { }

    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        this.buildForm();
        this.loadDetail(this.config.data.id);
        this.saveBtnName = 'Cập nhật';
        this.closeBtnName = 'Hủy';
    }

    noSpecial: RegExp = /^[^<>*!_~]+$/;
    validationMessages = {
        email: [
            { type: 'required', message: 'Bạn phải nhập email' },
            { type: 'email', message: 'Email không đúng định dạng' },
        ],
    };

    loadDetail(id: any){
        this.toggleBlockUI(true);
        this.userService.getUserById(id).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: UserDto) => {
                this.email = res.email;
                this.buildForm();
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    saveChange(){
        this.toggleBlockUI(true);
        this.saveData();
    }

    private saveData(){
        this.userService.changeEmail(this.config.data.id, this.form.value).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: () => {
                this.toggleBlockUI(false);
                this.ref.close(this.form.value);
            },
            error: (err) => {
                this.toggleBlockUI(false);
                console.log(err);
            }
        })
    }

    buildForm() {
        this.form = this.fb.group({
            email: new FormControl(this.email || null, Validators.compose([Validators.required, Validators.email]))
        })
    }

    private toggleBlockUI(enabled: boolean) {
        if (enabled == true) {
            this.btnDisabled = true;
            this.blockedPanelDetail = true;
        } else {
            setTimeout(() => {
                this.btnDisabled = false;
                this.blockedPanelDetail = false;
            }, 1000);
        }
    }
}