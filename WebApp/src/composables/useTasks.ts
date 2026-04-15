import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchTasks,
  createTask,
  deleteTask,
  updateTaskStatus,
} from "../services/taskApi";

export function useTasks(backlogId: string) {
  return useQuery({
    queryKey: ["tasks", backlogId],
    queryFn: async () => (await fetchTasks(backlogId)).data,
    enabled: !!backlogId,
  });
}

export function useCreateTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ backlogId, data }: { backlogId: string; data: any }) => createTask(backlogId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["tasks", variables.backlogId] });
    },
  });
}

export function useDeleteTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (taskId: string) => deleteTask(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
    },
  });
}

export function useUpdateTaskStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ taskId, status }: { taskId: string; status: string }) => updateTaskStatus(taskId, status),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
    },
  });
}
