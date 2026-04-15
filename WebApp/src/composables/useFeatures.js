import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchFeatures,
  fetchFeatureById,
  createFeature,
  deleteFeature,
  updateFeatureStatus,
} from "../services/featureApi";

export function useFeatures(projectId) {
  return useQuery({
    queryKey: ["features", projectId],
    queryFn: async () => (await fetchFeatures(projectId)).data,
    enabled: !!projectId,
  });
}

export function useFeature(featureId) {
  return useQuery({
    queryKey: ["feature", featureId],
    queryFn: async () => (await fetchFeatureById(featureId)).data,
    enabled: !!featureId,
  });
}

export function useCreateFeature() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ projectId, data }) => createFeature(projectId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries(["features", variables.projectId]);
    },
  });
}

export function useDeleteFeature() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (featureId) => deleteFeature(featureId),
    onSuccess: () => {
      queryClient.invalidateQueries(["features"]);
    },
  });
}

export function useUpdateFeatureStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ featureId, status }) => updateFeatureStatus(featureId, status),
    onSuccess: () => {
      queryClient.invalidateQueries(["features"]);
    },
  });
}
