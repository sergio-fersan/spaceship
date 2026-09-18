using UnityEngine;

namespace Spaceship
{
    /// <summary>
    /// Utilitario para descobrir os limites da tela em unidades de mundo
    /// (assumindo camera ortografica 2D).
    /// </summary>
    public static class ScreenBounds
    {
        private static Camera cache;

        public static Camera Cam
        {
            get
            {
                if (cache == null) cache = Camera.main;
                return cache;
            }
        }

        public static float Altura => Cam != null ? Cam.orthographicSize * 2f : 10f;
        public static float Largura => Cam != null ? Cam.orthographicSize * 2f * Cam.aspect : 17.78f;

        public static float Esquerda => (Cam != null ? Cam.transform.position.x : 0f) - Largura * 0.5f;
        public static float Direita => (Cam != null ? Cam.transform.position.x : 0f) + Largura * 0.5f;
        public static float Baixo => (Cam != null ? Cam.transform.position.y : 0f) - Altura * 0.5f;
        public static float Cima => (Cam != null ? Cam.transform.position.y : 0f) + Altura * 0.5f;
    }
}
