import { Component, OnDestroy, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { Subject, takeUntil } from 'rxjs';
import { AdminApiPostCategoryApiClient, PostCategoryDto, PostCategoryDtoPagedResult } from 'src/app/api/admin-api.service.generated';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
    selector: 'app-post-category',
    templateUrl: './post-category.component.html'
  })
export class PostCategoryComponent implements OnInit, OnDestroy {
    private ngUnsubscribe = new Subject<void>();
    public blockedPanel: boolean = false;

    //Paging variables
    public pageIndex: number = 1;
    public pageSize: number = 10;
    public totalCount: number;

    //Business variables
    public items: PostCategoryDto[];
    public selectedItems: PostCategoryDto[] = [];
    public keyword: string = '';

    constructor(private postCategoryService: AdminApiPostCategoryApiClient, public dialogService: DialogService, private alertService: AlertService, private confirmationService: ConfirmationService) { }
    ngOnDestroy(): void {
        this.ngUnsubscribe.next();
        this.ngUnsubscribe.complete();
    }
    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.toggleBlockUI(true);
        this.postCategoryService.getPostCategoriesPaging(this.keyword, this.pageIndex, this.pageSize).pipe(takeUntil(this.ngUnsubscribe)).subscribe({
            next: (res: PostCategoryDtoPagedResult) => {
                this.items = res.results;
                this.totalCount = res.rowCount;
                this.toggleBlockUI(false);
            },
            error: () => {
                this.toggleBlockUI(false);
            }
        })
    }

    showAddModal() { }
    showEditModal() { }
    deleteItems() { }
    deleteItemsConfirm(ids: any[]) { }

    pageChanged(event: any): void {
        this.pageIndex = event.page;
        this.pageSize = event.rows;
        this.loadData();
    }

    private toggleBlockUI(enabled: boolean) {
        if (enabled == true) {
            this.blockedPanel = true;
        }
        else {
            setTimeout(() => {
                this.blockedPanel = false;
            }, 1000);
        }
    }
}