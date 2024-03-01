import { UploadService } from './../../../shared/services/upload.service';
import { UtilityService } from './../../../shared/services/utility.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AdminApiPostApiClient, AdminApiPostCategoryApiClient, PostCategoryDto, PostDto } from './../../../api/admin-api.service.generated';
import { Component, EventEmitter, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { Subject, forkJoin, takeUntil } from "rxjs";
import { environment } from '../../../../environments/environment';
@Component({
    templateUrl: 'post-detail.component.html'
})
export class PostDetailComponent implements OnInit, OnDestroy {
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

    selectedEntity = {} as PostDto;
    public thumbnailImage;

    formSavedEventEmitter: EventEmitter<any> = new EventEmitter();

    constructor(public ref: DynamicDialogRef, public config: DynamicDialogConfig, private utilService: UtilityService, private postService: AdminApiPostApiClient, private postCategoryService: AdminApiPostCategoryApiClient, private uploadService: UploadService, private fb: FormBuilder) {

    }

    ngOnDestroy(): void {
        if (this.ref) {
            this.ngUnsubscribe.next();
            this.ngUnsubscribe.complete();
        }
    }
    ngOnInit(): void {
        this.buildForm();
        var categories = this.postCategoryService.getAllPostCategories();
        this.toggleBlockUI(true);
        forkJoin({ categories }).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (response: any) => {
                var categories = response.categories as PostCategoryDto[];
                categories.forEach(element => {
                    this.postCategories.push({
                        value: element.id,
                        label: element.name
                    });
                });
                if (this.utilService.isEmpty(this.config.data?.id) == false) {
                    this.loadFormDetails(this.config.data.id);
                } else {
                    this.toggleBlockUI(false);
                }
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
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

    loadFormDetails(id: string) {
        this.toggleBlockUI(true);
        this.postService.getPostById(id).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: PostDto) => {
                this.selectedEntity = res;
                this.buildForm();
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    onFileChange(event) {
        if (event.target.files && event.target.files.length) {
            this.uploadService.uploadImage('posts', event.target.files)
                .subscribe({
                    next: (response: any) => {
                        this.form.controls['thumbnail'].setValue(response.path);
                        this.thumbnailImage = environment.API_URL + response.path;
                    },
                    error: (err: any) => {
                        console.log(err);
                    }
                });
        }
    }

    saveChange() {
        this.saveData();
    }

    private saveData() {
        this.toggleBlockUI(true);
        if (this.utilService.isEmpty(this.config.data?.id)) {
            this.postService.createPost(this.form.value).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
                next: () => {
                    this.ref.close(this.form.value);
                    this.toggleBlockUI(false);
                },
                error: () => {
                    this.toggleBlockUI(false);
                }
            })
        } else {
            this.postService.updatePost(this.config.data.id, this.form.value).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
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

    buildForm() {
        this.form = this.fb.group({
            name: new FormControl(
                this.selectedEntity.name || null,
                Validators.compose([
                    Validators.required,
                    Validators.maxLength(255),
                    Validators.minLength(3),
                ])
            ),
            slug: new FormControl(this.selectedEntity.slug || null, Validators.required),
            categoryId: new FormControl(this.selectedEntity.categoryId || null, Validators.required),
            description: new FormControl(this.selectedEntity.description || null, Validators.required),
            seoDescription: new FormControl(this.selectedEntity.seoDescription || null),
            tags: new FormControl(this.selectedEntity.tags || null),
            content: new FormControl(this.selectedEntity.content || null),
            thumbnail: new FormControl(
                this.selectedEntity.thumbnail || null
            ),
        });
        if (this.selectedEntity.thumbnail) {
            this.thumbnailImage = environment.API_URL + this.selectedEntity.thumbnail;

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