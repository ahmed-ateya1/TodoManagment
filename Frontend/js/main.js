const API_BASE_URL = 'http://localhost:8080/api/Todo';

let currentPage = 1;
let pageSize = 10;
let totalPages = 1;
let todoToDelete = null;

window.openEditTodoModal = openEditTodoModal;
window.openDeleteModal = openDeleteModal;
window.markAsCompleted = markAsCompleted;
window.loadTodos = loadTodos;

const todoContainer = document.getElementById('todo-container');
const noTodosMessage = document.getElementById('no-todos-message');
const alertContainer = document.getElementById('alert-container');
const addTodoBtn = document.getElementById('add-todo-btn');
const todoModal = new bootstrap.Modal(document.getElementById('todo-modal'));
const deleteModal = new bootstrap.Modal(document.getElementById('delete-modal'));
const todoForm = document.getElementById('todo-form');
const saveTodoBtn = document.getElementById('save-todo-btn');
const confirmDeleteBtn = document.getElementById('confirm-delete-btn');
const statusFilter = document.getElementById('status-filter');
const priorityFilter = document.getElementById('priority-filter');
const dateFromFilter = document.getElementById('date-from');
const dateToFilter = document.getElementById('date-to');
const searchInput = document.getElementById('search-input');
const searchBtn = document.getElementById('search-btn');
const applyFiltersBtn = document.getElementById('apply-filters-btn');
const resetFiltersBtn = document.getElementById('reset-filters-btn');
const pagination = document.getElementById('pagination');
const spinnerOverlay = document.getElementById('spinner-overlay');

document.addEventListener('DOMContentLoaded', () => {
    loadTodos();

    addTodoBtn.addEventListener('click', openAddTodoModal);
    saveTodoBtn.addEventListener('click', saveTodo);
    confirmDeleteBtn.addEventListener('click', deleteTodo);
    searchBtn.addEventListener('click', searchTodos);
    applyFiltersBtn.addEventListener('click', () => loadTodos(1));
    resetFiltersBtn.addEventListener('click', resetFilters);

    searchInput.addEventListener('keyup', (e) => {
        if (e.key === 'Enter') {
            searchTodos();
        }
    });
});

function showSpinner() {
    spinnerOverlay.style.display = 'flex';
}

function hideSpinner() {
    spinnerOverlay.style.display = 'none';
}

function showAlert(message, type = 'success') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    alertContainer.appendChild(alertDiv);

    setTimeout(() => {
        alertDiv.classList.remove('show');
        setTimeout(() => alertDiv.remove(), 150);
    }, 5000);
}

async function loadTodos(page = 1) {
    showSpinner();
    currentPage = page;

    try {
        const params = new URLSearchParams();
        params.append('PageIndex', currentPage);
        params.append('PageSize', pageSize);

        if (statusFilter.value) {
            params.append('Status', statusFilter.value);
        }

        if (priorityFilter.value) {
            params.append('Priority', priorityFilter.value);
        }

        if (dateFromFilter.value) {
            params.append('DueDateFrom', new Date(dateFromFilter.value).toISOString());
        }

        if (dateToFilter.value) {
            params.append('DueDateTo', new Date(dateToFilter.value + 'T23:59:59').toISOString());
        }

        const response = await fetch(`${API_BASE_URL}/getAllTodos?${params}`);
        const data = await response.json();

        if (data.isSuccess) {
            renderTodos(data.result.items);
            renderPagination(data.result.totalPages, currentPage);
            totalPages = data.result.totalPages;
        } else {
            todoContainer.innerHTML = '';
            noTodosMessage.style.display = 'block';
            pagination.innerHTML = '';
        }
    } catch (error) {
        console.error('Error loading todos:', error);
        showAlert('Failed to load todos. Please try again later.', 'danger');
        noTodosMessage.style.display = 'block';
    } finally {
        hideSpinner();
    }
}

function renderTodos(todos) {
    todoContainer.innerHTML = '';

    if (!todos || todos.length === 0) {
        noTodosMessage.style.display = 'block';
        return;
    }

    noTodosMessage.style.display = 'none';

    todos.forEach(todo => {
        const dueDate = todo.dueDate ? new Date(todo.dueDate) : null;
        const isOverdue = dueDate && dueDate < new Date() && todo.status !== 'Completed';

        const todoCard = document.createElement('div');
        todoCard.className = 'col-md-6 col-lg-4 mb-4';
        todoCard.innerHTML = `
            <div class="card todo-card shadow-sm priority-${todo.priority.toLowerCase()} status-${todo.status.toLowerCase()}">
                <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <h5 class="card-title mb-0">${todo.title}</h5>
                        <span class="badge ${getPriorityBadgeClass(todo.priority)}">${todo.priority}</span>
                    </div>
                    <p class="card-text text-truncate">${todo.description || 'No description'}</p>
                    
                    <div class="mb-3">
                        <span class="badge bg-secondary">${todo.status}</span>
                        ${dueDate ? `
                            <span class="date-badge ms-2 ${isOverdue ? 'overdue' : ''}">
                                <i class="bi bi-calendar-event"></i> 
                                ${formatDate(dueDate)}
                                ${isOverdue ? ' (Overdue)' : ''}
                            </span>
                        ` : ''}
                    </div>
                    
                    <div class="btn-group w-100" role="group">
                        <button type="button" class="btn btn-sm btn-outline-primary" onclick="openEditTodoModal('${todo.id}')">
                            <i class="bi bi-pencil"></i> Edit
                        </button>
                        ${todo.status !== 'Completed' ? `
                            <button type="button" class="btn btn-sm btn-outline-success" onclick="markAsCompleted('${todo.id}')">
                                <i class="bi bi-check-lg"></i> Complete
                            </button>
                        ` : ''}
                        <button type="button" class="btn btn-sm btn-outline-danger" onclick="openDeleteModal('${todo.id}', '${todo.title.replace(/'/g, "\\'")}')">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </div>
                </div>
                <div class="card-footer text-muted small">
                    <i class="bi bi-clock"></i> Last updated: ${formatDateTime(new Date(todo.lastModifiedDate))}
                </div>
            </div>
        `;

        todoContainer.appendChild(todoCard);
    });
}

function renderPagination(totalPages, currentPage) {
    pagination.innerHTML = '';

    if (totalPages <= 1) {
        return;
    }

    const prevLi = document.createElement('li');
    prevLi.className = `page-item ${currentPage === 1 ? 'disabled' : ''}`;
    prevLi.innerHTML = `
        <a class="page-link" href="#" aria-label="Previous" ${currentPage > 1 ? `onclick="loadTodos(${currentPage - 1}); return false;"` : ''}>
            <span aria-hidden="true">«</span>
        </a>
    `;
    pagination.appendChild(prevLi);

    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);

    if (startPage > 1) {
        const firstLi = document.createElement('li');
        firstLi.className = 'page-item';
        firstLi.innerHTML = `<a class="page-link" href="#" onclick="loadTodos(1); return false;">1</a>`;
        pagination.appendChild(firstLi);

        if (startPage > 2) {
            const ellipsisLi = document.createElement('li');
            ellipsisLi.className = 'page-item disabled';
            ellipsisLi.innerHTML = `<a class="page-link" href="#">...</a>`;
            pagination.appendChild(ellipsisLi);
        }
    }

    for (let i = startPage; i <= endPage; i++) {
        const pageLi = document.createElement('li');
        pageLi.className = `page-item ${i === currentPage ? 'active' : ''}`;
        pageLi.innerHTML = `<a class="page-link" href="#" onclick="loadTodos(${i}); return false;">${i}</a>`;
        pagination.appendChild(pageLi);
    }

    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            const ellipsisLi = document.createElement('li');
            ellipsisLi.className = 'page-item disabled';
            ellipsisLi.innerHTML = `<a class="page-link" href="#">...</a>`;
            pagination.appendChild(ellipsisLi);
        }

        const lastLi = document.createElement('li');
        lastLi.className = 'page-item';
        lastLi.innerHTML = `<a class="page-link" href="#" onclick="loadTodos(${totalPages}); return false;">${totalPages}</a>`;
        pagination.appendChild(lastLi);
    }

    const nextLi = document.createElement('li');
    nextLi.className = `page-item ${currentPage === totalPages ? 'disabled' : ''}`;
    nextLi.innerHTML = `
        <a class="page-link" href="#" aria-label="Next" ${currentPage < totalPages ? `onclick="loadTodos(${currentPage + 1}); return false;"` : ''}>
            <span aria-hidden="true">»</span>
        </a>
    `;
    pagination.appendChild(nextLi);
}

function searchTodos() {
    const searchTerm = searchInput.value.trim();

    if (!searchTerm) {
        loadTodos(1);
        return;
    }

    showSpinner();

    fetch(`${API_BASE_URL}/getTodosByTitle/${encodeURIComponent(searchTerm)}?PageIndex=1&PageSize=${pageSize}`)
        .then(response => response.json())
        .then(data => {
            if (data.isSuccess) {
                renderTodos(data.result.items);
                renderPagination(data.result.totalPages, 1);
                totalPages = data.result.totalPages;
            } else {
                todoContainer.innerHTML = '';
                noTodosMessage.style.display = 'block';
                pagination.innerHTML = '';
            }
        })
        .catch(error => {
            console.error('Error searching todos:', error);
            showAlert('Failed to search todos. Please try again later.', 'danger');
        })
        .finally(() => {
            hideSpinner();
        });
}

function resetFilters() {
    statusFilter.value = '';
    priorityFilter.value = '';
    dateFromFilter.value = '';
    dateToFilter.value = '';
    searchInput.value = '';

    loadTodos(1);
}

function openAddTodoModal() {
    document.getElementById('todo-modal-label').textContent = 'Add New Todo';
    document.getElementById('todo-id').value = '';
    document.getElementById('todo-title').value = '';
    document.getElementById('todo-description').value = '';
    document.getElementById('todo-priority').value = 'Medium';
    document.getElementById('todo-due-date').value = '';

    document.getElementById('status-field-container').style.display = 'none';

    todoModal.show();
}

async function openEditTodoModal(todoId) {
    document.getElementById('todo-modal-label').textContent = 'Edit Todo';
    showSpinner();

    try {
        const response = await fetch(`${API_BASE_URL}/getTodo/${todoId}`);
        const data = await response.json();

        if (data.isSuccess) {
            const todo = data.result;
            document.getElementById('todo-id').value = todo.id;
            document.getElementById('todo-title').value = todo.title;
            document.getElementById('todo-description').value = todo.description;
            document.getElementById('todo-priority').value = todo.priority;
            document.getElementById('todo-status').value = todo.status;

            if (todo.dueDate) {
                const dueDate = new Date(todo.dueDate);
                document.getElementById('todo-due-date').value = formatDateForInput(dueDate);
            } else {
                document.getElementById('todo-due-date').value = '';
            }

            document.getElementById('status-field-container').style.display = 'block';

            todoModal.show();
        } else {
            showAlert('Failed to load todo details.', 'danger');
        }
    } catch (error) {
        console.error('Error loading todo details:', error);
        showAlert('Failed to load todo details. Please try again later.', 'danger');
    } finally {
        hideSpinner();
    }
}

function openDeleteModal(todoId, todoTitle) {
    todoToDelete = todoId;
    document.getElementById('delete-todo-title').textContent = todoTitle;
    deleteModal.show();
}

async function saveTodo() {
    const todoId = document.getElementById('todo-id').value;
    const title = document.getElementById('todo-title').value.trim();
    const description = document.getElementById('todo-description').value.trim();
    const priority = document.getElementById('todo-priority').value;
    const dueDate = document.getElementById('todo-due-date').value;

    if (!title) {
        document.getElementById('todo-title').classList.add('is-invalid');
        return;
    } else {
        document.getElementById('todo-title').classList.remove('is-invalid');
    }

    showSpinner();

    try {
        let url, method, todoData;

        if (todoId) {
            url = `${API_BASE_URL}/updateTodo`;
            method = 'PUT';
            todoData = {
                id: todoId,
                title: title,
                description: description,
                status: document.getElementById('todo-status').value,
                priority: priority,
                dueDate: dueDate ? new Date(dueDate).toISOString() : null
            };
        } else {
            url = `${API_BASE_URL}/addTodo`;
            method = 'POST';
            todoData = {
                title: title,
                description: description,
                priority: priority,
                dueDate: dueDate ? new Date(dueDate).toISOString() : null
            };
        }

        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(todoData)
        });

        const data = await response.json();

        if (data.isSuccess) {
            todoModal.hide();
            showAlert(todoId ? 'Todo updated successfully!' : 'Todo created successfully!');
            loadTodos(currentPage);
        } else {
            showAlert(data.message || 'Failed to save todo.', 'danger');
        }
    } catch (error) {
        console.error('Error saving todo:', error);
        showAlert('Failed to save todo. Please try again later.', 'danger');
    } finally {
        hideSpinner();
    }
}

async function deleteTodo() {
    if (!todoToDelete) return;

    showSpinner();

    try {
        const response = await fetch(`${API_BASE_URL}/deleteTodo/${todoToDelete}`, {
            method: 'DELETE'
        });

        const data = await response.json();

        if (data.isSuccess) {
            deleteModal.hide();
            showAlert('Todo deleted successfully!');

            const reloadPage = (currentPage > 1 && document.querySelectorAll('.todo-card').length === 1)
                ? currentPage - 1
                : currentPage;

            loadTodos(reloadPage);
        } else {
            showAlert(data.message || 'Failed to delete todo.', 'danger');
        }
    } catch (error) {
        console.error('Error deleting todo:', error);
        showAlert('Failed to delete todo. Please try again later.', 'danger');
    } finally {
        hideSpinner();
        todoToDelete = null;
    }
}

async function markAsCompleted(todoId) {
    showSpinner();

    try {
        const response = await fetch(`${API_BASE_URL}/marktodoComplete/${todoId}`, {
            method: 'PATCH'
        });

        const data = await response.json();

        if (data.isSuccess) {
            showAlert('Todo marked as completed!');
            loadTodos(currentPage);
        } else {
            showAlert(data.message || 'Failed to mark todo as completed.', 'danger');
        }
    } catch (error) {
        console.error('Error marking todo as completed:', error);
        showAlert('Failed to mark todo as completed. Please try again later.', 'danger');
    } finally {
        hideSpinner();
    }
}

// Helper functions
function getPriorityBadgeClass(priority) {
    switch (priority) {
        case 'Low':
            return 'bg-success';
        case 'Medium':
            return 'bg-warning text-dark';
        case 'High':
            return 'bg-danger';
        default:
            return 'bg-secondary';
    }
}

function formatDate(date) {
    if (!date) return '';
    return date.toLocaleDateString();
}

function formatDateTime(date) {
    if (!date) return '';
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}

function formatDateForInput(date) {
    if (!date) return '';
    return date.toISOString().slice(0, 16);
}