import { UploadService } from './../../../shared/services/upload.service';
import { UtilityService } from './../../../shared/services/utility.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AdminApiSeriesApiClient, SeriesDto } from './../../../api/admin-api.service.generated';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Component, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { environment } from '../../../../environments/environment';
@Component({
    templateUrl: 'series-detail.component.html'
})
export class SeriesDetailComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();

    // Default
    public blockedPanelDetail: boolean = false;
    public form: FormGroup;
    public title: string;
    public btnDisabled = false;
    public saveBtnName: string;
    public postCategories: any[] = [];
    public contentTypes: any[] = [];
    public series: any[] = [];

    selectedEntity = {} as SeriesDto;
    public thumbnailImage;

    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(public ref: DynamicDialogRef,
        public config: DynamicDialogConfig,
        private utilService: UtilityService,
        private fb: FormBuilder,
        private seriesApiClient: AdminApiSeriesApiClient,
        private uploadService: UploadService) { }

    ngOnDestroy(): void {
        if (this.ref) {
            this.ref.close();
        }
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        throw new Error('Method not implemented.');
    }

    public generateSlug() {
        var slug = this.utilService.makeSeoTitle(this.form.get('name')?.value);
        this.form.controls['slug'].setValue(slug);
    }
    // Validate
    noSpecial: RegExp = /^[^<>*!_~]+$/;
    validationMessages = {
        name: [
            { type: 'required', message: 'Bạn phải nhập tên' },
            { type: 'minlength', message: 'Bạn phải nhập ít nhất 3 kí tự' },
            { type: 'maxlength', message: 'Bạn không được nhập quá 255 kí tự' },
        ],
        slug: [{ type: 'required', message: 'Bạn phải URL duy nhất' }],
        description: [{ type: 'required', message: 'Bạn phải nhập mô tả ngắn' }],
    };

    buildForm() {
        this.form = this.fb.group({
            name: new FormControl(this.selectedEntity.name || null, Validators.compose([Validators.required, Validators.maxLength(255), Validators.minLength(3)])),
            slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
            description: new FormControl(this.selectedEntity.description || null, Validators.required),
            seoDescription: new FormControl(this.selectedEntity.seoDescription || null),
            content: new FormControl(this.selectedEntity.content || null),
            isActive: new FormControl(this.selectedEntity.isActive || null),
            thumbnail: new FormControl(
                this.selectedEntity.thumbnail || null
            ),
        })
        if (this.selectedEntity.thumbnail) {
            this.thumbnailImage = environment.API_URL + this.selectedEntity.thumbnail;
        }
    }

    loadFormDetails(id: any) {
        this.toggleBlockUI(true);
        this.seriesApiClient.getAllSeriesById(id).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: SeriesDto) => {
                this.selectedEntity = res;
                this.buildForm();
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    onFileChange(event) { }

    saveChange() {
        this.toggleBlockUI(true);
        this.saveData();
    }

    private saveData() {
        this.toggleBlockUI(true);
        if (this.utilService.isEmpty(this.config.data.id)) {
            this.seriesApiClient.createSeries(this.form.value).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
                next: () => {
                    this.ref.close();
                    this.toggleBlockUI(false);
                },
                error: () => {
                    this.toggleBlockUI(false);
                }
            })
        } else {
            this.seriesApiClient.updateSeries(this.config.data.id, this.form.value).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
                next: () => {
                    this.ref.close(this.form.value);
                    this.toggleBlockUI(false);
                },
                error: () => {
                    this.toggleBlockUI(false);
                }
            })
        }
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