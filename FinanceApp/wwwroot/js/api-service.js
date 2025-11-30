vclass ApiService {
    async fetchWithAuth(url, options = {}) {
        let response = await fetch(url, {
            ...options,
            credentials: 'include', // Important: sends cookies
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            }
        });

        // If unauthorized, try to refresh
        if (response.status === 401) {
            const refreshed = await this.refreshToken();

            if (refreshed) {
                // Retry the original request
                response = await fetch(url, {
                    ...options,
                    credentials: 'include',
                    headers: {
                        'Content-Type': 'application/json',
                        ...options.headers
                    }
                });
            } else {
                // Refresh failed, redirect to login
                window.location.href = '/login';
            }
        }

        return response;
    }

    async refreshToken() {
        try {
            const response = await fetch('https://localhost:5001/refresh', {
                method: 'POST',
                credentials: 'include'
            });
            return response.ok;
        } catch {
            return false;
        }
    }

    // Helper methods for common operations
    async get(url) {
        return this.fetchWithAuth(url, { method: 'GET' });
    }

    async post(url, data) {
        return this.fetchWithAuth(url, {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async put(url, data) {
        return this.fetchWithAuth(url, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async delete (url) {
        return this.fetchWithAuth(url, { method: 'DELETE' });
    }
}

// Export a singleton instance
const apiService = new ApiService();