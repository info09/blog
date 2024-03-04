import { AdminApiPostApiClient } from './../../../api/admin-api.service.generated';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
@Component({
    templateUrl: 'post-return-reason.component.html'
})
export class PostReturnReasonComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();
    // Default
    public blockedPanelDetail: boolean = false;
    public form: FormGroup;
    public title: string;
    public btnDisabled = false;
    public saveBtnName: string;
    public contentTypes: any[] = [];

    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig, private fb: FormBuilder, private postService: AdminApiPostApiClient) {

    }
    ngOnDestroy(): void {
        if(this.ref){
            this.ref.close();
        }
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        this.buildForm();
    }

    validationMessages = {
        reason: [{ type: 'required', message: 'Bạn phải nhập lý do' }],
    };

    saveChange() {
        this.toggleBlockUI(true);
        this.saveData();
    }

    saveData(){
        this.toggleBlockUI(true);
        this.postService.returnBack(this.config.data.id, this.form.value).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: () => {
                this.ref.close(this.form.value);
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    buildForm() {
        this.form = this.fb.group({
            reason: new FormControl(null, Validators.required)
        });
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